using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using DevSpaceWeb.Services;
using Microsoft.AspNetCore.Components.Web;
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
    public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    public static string GetVersionText()
    {
        if (Version.StartsWith("0."))
            return Version + " Dev";
        return Version + " Release";
    }

    public static InteractiveServerRenderMode RenderMode = new InteractiveServerRenderMode(prerender: false);

    /// <summary>
    /// Current directory of the running program
    /// </summary>
    public static DirectoryStructureMain Directory;

    public static HttpClient Http = new HttpClient();

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

    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ShowDemoLink = builder.Configuration.GetValue<bool>("ShowDemoLink");
        IsDevMode = Environment.GetEnvironmentVariable("DEVSPACE") == "Development";
        IsPreviewMode = Environment.GetEnvironmentVariable("PREVIEW") == "true";

        //WebRequest.DefaultWebProxy = new WebProxy("127.0.0.1", 8888);
        Logger.RunLogger("Dev Space", LogSeverity.Debug);

        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config file.");

        Logger.LogMessage("Loaded config in: " + Program.Directory.Path, LogSeverity.Info);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        // Rcon support
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);



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


        _DB.Init(builder.Configuration);

        _ = Task.Run(async () =>
        {
            _DB.IsConnected = await _DB.StartAsync();
        });


        // Add services to the container.
        ServiceBuilder.Build(builder, builder.Services);


        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
        });
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddRazorPages();
        builder.Services.AddControllers(opt =>
        {
            opt.RequireHttpsPermanent = true;
            opt.Filters.Add<ControllerExceptionFilter>();
        });
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
                    c.ShowCommonExtensions();
                    c.DocumentTitle = "Dev Space Endpoints";
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post);
                    c.ConfigObject = new ConfigObject
                    {
                        Urls = new List<UrlDescriptor>
                        {
                            new UrlDescriptor() { Url = "/swagger/client/swagger.json", Name = "Client API" },
                            new UrlDescriptor() { Url = "/swagger/instance/swagger.json", Name = "Instance API" },
                        },
                        //DefaultModelExpandDepth = 2,
                        DefaultModelRendering = ModelRendering.Example,
                        DefaultModelsExpandDepth = -1,
                        DisplayOperationId = false
                    };
                    //c.EnableFilter();
                });
            }
            app.MapSwagger();
        }

        //new InfoTest().Run();
        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();
        app.MapRazorPages();
        app.MapControllers();
        app.Run();
    }
}