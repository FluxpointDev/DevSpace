using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using DevSpaceWeb.Services;
using Docker.DotNet;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DevSpaceWeb;
public class Program
{
    /// <summary>
    /// Version of DevSpaceWeb.
    /// </summary>
    public static string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);

    public static string? LatestWebVersion;
    public static string? LatestAgentVersion;

    public static string GetVersionText()
    {
        if (Version.StartsWith("1."))
            return Version + " Beta";
        return Version + " Release";
    }

    public static InteractiveServerRenderMode RenderMode = new InteractiveServerRenderMode(prerender: false);

    /// <summary>
    /// Current directory of the running program
    /// </summary>
    public static DirectoryStructureMain Directory;

    public static HttpClient Http = new HttpClient();

    public static HttpClient AgentDiscoverHttp = new HttpClient(new HttpClientHandler
    {
        AllowAutoRedirect = false,
        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            if (cert != null || (cert != null && cert.Subject == "CN=devspace"))
                return true;

            return false;
        }
    });

    public static IServiceCollection Services;

    public static MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

    /// <summary>
    /// Program is running in development
    /// </summary>
    public static bool IsDevMode { get; private set; }

    public static bool IsPreviewMode { get; set; } = false;
    public static bool ShowDemoLink { get; set; } = false;

    public static Regex IpRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", RegexOptions.Compiled | RegexOptions.Multiline);

    public static string PublicIP { get; private set; }

    public static DockerClient InternalDocker;

    public static bool LimitMode = false;

    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ShowDemoLink = builder.Configuration.GetValue<bool>("ShowDemoLink");
        IsDevMode = Environment.GetEnvironmentVariable("DEVSPACE") == "Development";
        IsPreviewMode = Environment.GetEnvironmentVariable("PREVIEW") == "true";
        LimitMode = Environment.GetEnvironmentVariable("LimitMode") == "true";
        string? LogMode = Environment.GetEnvironmentVariable("LogMode");

        LogSeverity DefaultLogMode = LogSeverity.Error;
        if (IsDevMode)
            DefaultLogMode = LogSeverity.Debug;
        else
        {
            if (!string.IsNullOrEmpty(LogMode))
            {
                Enum.TryParse(LogMode, out DefaultLogMode);
            }
        }

        // TODO import portainer yml files
        //string YmlText = "";
        //var deserializer = new DeserializerBuilder().Build();
        //var yamlObject = deserializer.Deserialize(YmlText);

        //var serializer = new SerializerBuilder()
        //    .JsonCompatible()
        //    .Build();

        //var json = serializer.Serialize(yamlObject);
        //var JO = JObject.Parse(json);
        //Console.WriteLine(JO["services"].First().Path.Replace("services.", ""));

        //WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);
        Logger.RunLogger("Dev Space", DefaultLogMode);
        Logger.LogMessage("Dev Space v" + GetVersionText(), LogSeverity.Info);

        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config file.");

        Logger.LogMessage("Loaded config in: " + Program.Directory.Path, LogSeverity.Info);

        // Guid support for mongodb
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        if (!LimitMode)
        {
            // Rcon support
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        Logger.LogMessage("Running connection test...", LogSeverity.Info);

        // Don't run connection test in development
        if (!IsDevMode)
        {
            bool ConnectionTest = false;
            try
            {
                string IPString = await Http.GetStringAsync("https://icanhazip.com");
                IPString = IPString.Trim();
                if (!IPString.Contains("."))
                    Logger.LogMessage("Connection test invalid response", LogSeverity.Info);
                else
                    ConnectionTest = true;

                PublicIP = IPString;
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Connection test error " + ex.Message, LogSeverity.Info);
            }

            if (ConnectionTest)
                Logger.LogMessage("Connection test success", LogSeverity.Info);
            else
            {
                throw new Exception("Connection test failed");
            }
        }

        if (Program.IsDevMode)
        {
            InternalDocker = new DockerClientConfiguration(new Uri("tcp://127.0.0.1:2375"), null).CreateClient();
            try
            {
                DriveInfo CurrentDrive = DriveInfo.GetDrives().First();

                string Used = Utils.SizeSuffix(CurrentDrive.TotalSize - CurrentDrive.TotalFreeSpace, 0);
                string Total = Utils.SizeSuffix(CurrentDrive.TotalSize, 0);


                await InternalDocker.System.PingAsync();
                Console.WriteLine("PINGED DOCKER");

                //await InternalDocker.Images.CreateImageAsync(new ImagesCreateParameters
                //{
                //    FromImage = "docker.io/aquasec/trivy",
                //    Tag = "latest"
                //}, null, new Progress<JSONMessage>());

                //await InternalDocker.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters
                //{
                //    Cmd = new List<string>
                //    {
                //        "filesystem",
                //        "/root/mount/",
                //        "--skip-files",
                //        "-q",
                //        "--no-progress",
                //        "--format",
                //        "json",
                //        "--security-checks",
                //        "vuln"
                //    },
                //    Image = "docker.io/aquasec/trivy",
                //    Name = "security-scan-" + Guid.NewGuid().ToString().Replace("-", ""),
                //    HostConfig = new HostConfig
                //    {
                //        Binds = new List<string>
                //        {
                //            "/var/trivy:/root/.cache:rw",
                //            "/root/services/devspace_agent:/root/mount/root/services/devspace_agent:ro"
                //        }
                //    }
                //});

                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Version, Newtonsoft.Json.Formatting.Indented));
                //var Procs = await InternalDocker.Containers.ListProcessesAsync("d4718c5b63aaab415034b1f1ef99ca24859b388fca474cdf8f461e387c78a192", new Docker.DotNet.Models.ContainerListProcessesParameters
                //{
                //    PsArgs = "-eo user,pid,ppid,thcount,c,%cpu,%mem,lstart,etime,comm,cmd --date-format %Y-%m-%dT%H:%M:%S"
                //});
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject(Data), Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed " + ex.Message);
            }
        }


        _DB.Init(builder.Configuration);

        _ = Task.Run(async () =>
        {
            _DB.IsConnected = await _DB.StartAsync();
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedPrefix | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 2;
            options.KnownProxies.Clear();
            options.KnownNetworks.Clear();
        });


        // Add services to the container.
        ServiceBuilder.Build(builder, builder.Services);

        Services = builder.Services;

        WebApplication app = builder.Build();


        // Configure the HTTP request pipeline.
        //if (!app.Environment.IsDevelopment())
        //{
        //    //app.UseExceptionHandler("/Error");
        //}
        app.UseForwardedHeaders();

        // Use both wwwroot and public folder for website access :)
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Program.Directory.Public.Path),
            RequestPath = "/public",
            OnPrepareResponse = async ctx =>
            {
                Logger.LogMessage("Got Response", LogSeverity.Debug);
                if (!(_Data.Config.Instance.Features.AllowUnauthenticatedPublicFolderAccess || (ctx.Context.User.Identity != null || ctx.Context.User.Identity.IsAuthenticated)))
                {
                    ctx.Context.Response.Clear();
                    ctx.Context.Response.StatusCode = 400;
                    ctx.Context.Response.ContentLength = 0;
                    ctx.Context.Response.Body = Stream.Null;
                    await ctx.Context.Response.CompleteAsync();
                }
            }
        });

        app.MapHealthChecks("/api/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckService.WriteResponse
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.UseAntiforgery();
        if (_Data.Config.Instance.Features.SwaggerEnabled)
        {
            Logger.LogMessage("Swagger Enabled", LogSeverity.Info);
            app.UseSwagger(c =>
            {

            });

            if (_Data.Config.Instance.Features.SwaggerUIEnabled)
            {
                Logger.LogMessage("Swagger UI Enabled", LogSeverity.Info);
                app.UseSwaggerUI(c =>
                {
                    c.DocumentTitle = "Dev Space API";
                    c.ConfigObject = new ConfigObject
                    {
                        Urls = new List<UrlDescriptor>
                        {
                            new UrlDescriptor() { Url = "/swagger/client/swagger.json", Name = "Client API" }
                        },
                        DefaultModelRendering = ModelRendering.Example,
                        DefaultModelsExpandDepth = -1,
                        Filter = ""
                    };
                });
                app.MapSwagger();
            }

        }


        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        if (!LimitMode)
            app.MapRazorPages();

        app.MapControllers();
        app.Run();
    }
}