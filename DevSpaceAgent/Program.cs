using DevSpaceAgent.Data;
using DevSpaceAgent.Json;
using DevSpaceAgent.Server;
using DevSpaceShared;
using DevSpaceShared.Data;
using Docker.DotNet;
using Docker.DotNet.Models;
using NetCoreServer;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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
    static async Task Main(string[] args)
    {
        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config data.");
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


        Console.WriteLine("[Agent] Starting websocket server");

        SslContext context = new SslContext(SslProtocols.None, Certificate);
        context.CertificateValidationCallback = (x, s, t, b) => { return true; };
        Server = new AgentServer(context, IPAddress.Any, 5555);

        if (!Server.Start())
            throw new Exception("Failed to start websocket server.");

        Console.WriteLine("[Agent] Loaded websocket server");

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
        Server.MulticastText(JsonConvert.SerializeObject(Stats));
    }

    public static System.Timers.Timer timer = new System.Timers.Timer(new TimeSpan(0, 5, 0));

    private static void Progress_ProgressChanged(object? sender, Message e)
    {
        if (e.Action.EndsWith("healthcheck") || e.Action == "exec_die")
            return;
        Console.WriteLine("--- Docker Message ---");
        Console.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented));
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
                            State.CurrentPullLimit -= 1;
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
