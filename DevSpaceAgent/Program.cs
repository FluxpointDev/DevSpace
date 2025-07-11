using DevSpaceAgent.Data;
using DevSpaceAgent.Json;
using DevSpaceAgent.Server;
using DevSpaceShared;
using DevSpaceShared.Agent;
using DevSpaceShared.Data;
using DevSpaceShared.Services;
using Docker.DotNet;
using Docker.DotNet.Models;
using NetCoreServer;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;

namespace DevSpaceAgent;

public class Program
{
    /// <summary>
    /// Version of DevSpace Agent.
    /// </summary>
    public static string? Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);

    public static string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

    public static DockerClient? DockerClient;

    public static X509Certificate2? Certificate;

    public static bool DockerFailed = false;

    public static AgentState State = new AgentState();

    public static System.Net.Http.HttpClient UnAuthenticatedClient = new System.Net.Http.HttpClient();

    public static Dictionary<string, DockerCustomTemplate> CustomTemplates = [];

    public static void SaveTemplates()
    {
        using (StreamWriter file = File.CreateText(CurrentDirectory + $"Data/Templates.json"))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, CustomTemplates);
        }
    }

    public static void SaveStacks()
    {
        using (StreamWriter file = File.CreateText(CurrentDirectory + $"Data/Stacks.json"))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, Stacks);
        }
    }

    public static Dictionary<string, string> ContainerCache = [];
    public static Dictionary<string, StackFile> Stacks = [];
    private static AgentServer Server;
    private static EdgeClient Client;
    public static bool EdgeMode;
    static async Task Main(string[] args)
    {
        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config data.");

        Logger.RunLogger("Dev Space Agent", LogSeverity.Error);

        try
        {
            DockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
            await DockerClient.System.PingAsync();

            _ = Task.Run(async () =>
            {
                foreach (ContainerListResponse? c in await DockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true
                }))
                {
                    ContainerCache.TryAdd(c.ID, c.Names.Any() ? c.Names.First().Substring(1) : c.ID);
                }
            });

            Console.WriteLine("[Docker] Connected to socket");
            _ = Task.Run(async () =>
            {
                CancellationTokenSource cancellation = new CancellationTokenSource();
                Progress<Message> Progress = new Progress<Message>();
                Progress.ProgressChanged += Progress_ProgressChanged;
                await DockerClient.System.MonitorEventsAsync(new ContainerEventsParameters(), Progress, cancellation.Token);

            });
        }
        catch
        {
            DockerFailed = true;
        }

        if (DockerFailed)
            Console.WriteLine("[Docker] Failed to connect to socket!");

        // Cleanup Dev Space images.
        _ = Task.Run(async () =>
        {
            IList<ImagesListResponse> Images = await DockerClient.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            });

            foreach (ImagesListResponse? i in Images)
            {
                if (i.RepoTags != null && i.RepoTags.Any())
                    continue;

                if (i.RepoDigests != null && i.RepoDigests.Any() && i.RepoDigests.First().StartsWith("ghcr.io/fluxpointdev/devspace"))
                {
                    try
                    {
                        await DockerClient.Images.DeleteImageAsync(i.ID, new ImageDeleteParameters
                        {

                        });
                    }
                    catch { }
                }
            }
        });

        if (!DockerFailed)
        {
            try
            {
                DockerAuthJson? Auth = await UnAuthenticatedClient.GetFromJsonAsync<DockerAuthJson>("https://auth.docker.io/token?service=registry.docker.io&scope=repository:ratelimitpreview/test:pull");
                if (Auth != null)
                {
                    HttpRequestMessage Req = new HttpRequestMessage(HttpMethod.Get, "https://registry-1.docker.io/v2/ratelimitpreview/test/manifests/latest");
                    Req.Headers.Add("Authorization", "Bearer " + Auth.token);
                    HttpResponseMessage Status = await UnAuthenticatedClient.SendAsync(Req);
                    if (Status.Headers.TryGetValues("ratelimit-limit", out IEnumerable<string>? values))
                        State.MaxPullLimit = int.Parse(values.First().Split(";").First());
                    if (Status.Headers.TryGetValues("ratelimit-remaining", out values))
                        State.CurrentPullLimit = int.Parse(values.First().Split(";").First());
                }
            }
            catch
            {

            }
        }

        string? EdgeHost = Environment.GetEnvironmentVariable("EDGE_HOST");
        if (!string.IsNullOrEmpty(EdgeHost))
        {
            string? EdgeId = Environment.GetEnvironmentVariable("EDGE_ID");
            string? EdgeToken = Environment.GetEnvironmentVariable("EDGE_KEY");
            if (!string.IsNullOrEmpty(EdgeId))
                _Data.Config.EdgeId = EdgeId;

            if (!string.IsNullOrEmpty(EdgeToken))
                _Data.Config.EdgeKey = EdgeToken;

            string[] Split = EdgeHost.Replace("http://", "", StringComparison.OrdinalIgnoreCase).Replace("https://", "", StringComparison.OrdinalIgnoreCase).Split(":");

            _Data.Config.EdgeIp = Split[0];
            if (Split.Length == 2 && short.TryParse(Split[1], out short port))
                _Data.Config.EdgePort = port;
            else
                _Data.Config.EdgePort = 443;

            if (string.IsNullOrEmpty(_Data.Config.EdgeId))
            {
                string? EdgeTeam = Environment.GetEnvironmentVariable("EDGE_TEAM");
                string? EdgeOnboard = Environment.GetEnvironmentVariable("EDGE_ONBOARD");

                if (!string.IsNullOrEmpty(EdgeTeam) && !string.IsNullOrEmpty(EdgeOnboard))
                {
                    string ServerName = System.Environment.MachineName;
                    if (!DockerFailed && DockerClient != null)
                    {
                        try
                        {
                            SystemInfoResponse SystemInfo = await DockerClient.System.GetSystemInfoAsync();
                            ServerName = SystemInfo.Name;
                        }
                        catch { }
                    }

                    try
                    {
                        System.Net.Http.HttpClient Http = new System.Net.Http.HttpClient();
                        HttpResponseMessage Response = await Http.SendAsync(new HttpRequestMessage(HttpMethod.Post, $"https://{_Data.Config.EdgeIp}:{_Data.Config.EdgePort}/edge/onboard")
                        {
                            Content = JsonContent.Create(new AgentOnboardCreate
                            {
                                ServerName = ServerName,
                                EdgeTeam = EdgeTeam,
                                EdgeOnboardKey = EdgeOnboard
                            })
                        });
                        Response.EnsureSuccessStatusCode();

                        AgentOnboardResponse? Onboard = await Response.Content.ReadFromJsonAsync<AgentOnboardResponse>();
                        if (Onboard != null)
                        {
                            _Data.Config.EdgeId = Onboard.EdgeId;
                            _Data.Config.EdgeKey = Onboard.EdgeKey;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to onboard");
                        Console.WriteLine(ex);
                    }
                }
            }

            _Data.Config.Save();
        }
        EdgeMode = !string.IsNullOrEmpty(_Data.Config.EdgeIp);

        Console.WriteLine("[Agent] Starting with " + (EdgeMode ? "edge mode" : "websocket mode"));

        if (EdgeMode)
        {
            if (string.IsNullOrEmpty(_Data.Config.EdgeId))
                throw new Exception("Edge mode is missing the edge id.");

            IPAddress? address = null;
            if (!IPAddress.TryParse(_Data.Config.EdgeIp, out address))
            {
                IPHostEntry? Host = null;
                try
                {
                    Host = Dns.GetHostEntry(_Data.Config.EdgeIp);
                }
                catch { }
                if (Host != null)
                    address = Host.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
            }

            Console.WriteLine("Connecting to: " + address.ToString());

            Client = new EdgeClient(_Data.Config.EdgeIp, _Data.Config.EdgePort, _Data.Config.EdgeId, _Data.Config.EdgeKey);
            await Client.Connect("", 0, "", true);
        }
        else
        {
            SslContext context = new SslContext(SslProtocols.None, Certificate);
            context.CertificateValidationCallback = (x, s, t, b) => { return true; };
            Server = new AgentServer(context, IPAddress.Any, 5555);

            if (!Server.Start())
                throw new Exception("Failed to start websocket server.");

            Console.WriteLine("[Agent] Loaded websocket server");
        }

        timer.Elapsed += new ElapsedEventHandler(RunChecks);
        timer.Start();

        await Task.Delay(-1);
    }

    public static async void RunChecks(object? sender, ElapsedEventArgs? e)
    {
        if (DockerClient == null || Server == null)
            return;

        SystemInfoResponse HostInfo = await DockerClient.System.GetSystemInfoAsync();
        AgentStatsResponse Stats = await AgentStatsResponse.Create(Program.Version, Program.DockerClient, HostInfo);
        if (Server != null)
            Server.MulticastText(System.Text.Json.JsonSerializer.Serialize(Stats, AgentJsonOptions.Options));
        else if (Client != null)
            _ = Client.WebSocket.SendAsync(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(Stats, AgentJsonOptions.Options)), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public static System.Timers.Timer timer = new System.Timers.Timer(new TimeSpan(0, 5, 0));

    private static void Progress_ProgressChanged(object? sender, Message e)
    {
        if (e.Action.EndsWith("healthcheck") || e.Action.StartsWith("exec_die") || e.Action.StartsWith("exec_create") || e.Action.StartsWith("exec_start"))
            return;

        if (_Data.Config.Options.LogDockerEvents)
        {
            Console.WriteLine("--- Docker Event ---");
            Console.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented));
            Console.WriteLine("--- --- --- --- ---");
        }
        switch (e.Type)
        {
            case "container":
                if (e.Action == "rename")
                {
                    if (ContainerCache.ContainsKey(e.ID))
                    {
                        ContainerCache[e.ID] = e.Actor.Attributes["name"];
                    }
                }
                else if (e.Action == "destroy")
                {
                    ContainerCache.Remove(e.ID);
                }
                else if (e.Action == "create")
                {
                    if (!ContainerCache.TryAdd(e.ID, e.Actor.Attributes["name"]))
                    {
                        ContainerCache[e.ID] = e.Actor.Attributes["name"];
                    }
                }
                break;
            case "image":
                switch (e.Action)
                {
                    case "pull":
                        if (e.Scope == "local")
                        {
                            if (e.ID.Count(x => x == '/') < 3)
                                State.CurrentPullLimit -= 1;
                        }
                        break;
                }
                break;
            case "plugin":
                break;
            case "volume":
                break;
            case "network":
                break;
            case "daemon":
                break;
        }
    }
}