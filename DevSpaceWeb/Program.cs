using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using DevSpaceWeb.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DevSpaceWeb;
public class Program
{

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

    public static void Main(string[] args)
    {
        Logger.RunLogger("Dev Space", LogSeverity.Error);
        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config file.");

        Logger.LogMessage("Loaded config in: " + Program.Directory.Path, LogSeverity.Info);

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IsDevMode = Environment.GetEnvironmentVariable("DEVSPACE") == "Development";
        IsPreviewMode = Environment.GetEnvironmentVariable("PREVIEW") == "true";
        _DB.Client = new MongoDB.Driver.MongoClient(_Data.Config.Database.GetConnectionString());
        _DB.Init();
        

        if (_Data.Config.Database.IsSetup)
        {
            _ = Task.Run(async () =>
            {
                await _DB.StartAsync();

                while (!_DB.IsConnected)
                {

                }
            });
        }

        // Add services to the container.
        ServiceBuilder.Build(builder, builder.Services);

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
        });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddControllers();

        Services = builder.Services;

        WebApplication app = builder.Build();


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

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
                    c.DocumentTitle = "Dev Space Endpoints";
                    c.SwaggerEndpoint("v1/swagger.json", "Dev Space API");
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post);
                    c.ConfigObject = new ConfigObject
                    {
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
        app.MapControllers();
        app.Run();
    }
}
