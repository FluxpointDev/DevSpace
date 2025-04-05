using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.Slack;
using AspNetCore.Identity.MongoDbCore;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using DevSpaceWeb.Fido2;
using DevSpaceWeb.Models.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Radzen;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace DevSpaceWeb.Services;

public static class ServiceBuilder
{
    public static void Build(WebApplicationBuilder builder, IServiceCollection services)
    {

        HealthCheckService HealthService = new HealthCheckService();

        // Add HTTP access
        services.AddHttpContextAccessor();
        services.AddScoped<HttpContextAccessor>();

        services.AddSingleton(new BackgroundTasks());
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddRadzenComponents();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(2);
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = "DevSpace.Session";
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        AddHealth(services, HealthService);
        AddFido2(services);
        AddIdentity(services);
        AddMongoDb(services);
        AddEmail(services, HealthService);
        AddProviders(services);

        if (_Data.Config.Instance.Features.SwaggerEnabled)
            AddSwagger(services);

        AddPages(services);
    }

    public static void AddPages(IServiceCollection services)
    {
        //services.AddControllers(opt =>
        //{
        //    opt.RequireHttpsPermanent = true;
        //    opt.Filters.Add<ControllerExceptionFilter>();
        //});

        services.AddRazorComponents(opt =>
        {
            opt.DetailedErrors = Program.IsDevMode;
        }).AddInteractiveServerComponents(opt =>
            {
                opt.DetailedErrors = Program.IsDevMode;
            });


        var MvcBuilder = services.AddMvcCore(opt =>
        {
            opt.RequireHttpsPermanent = true;
            opt.Filters.Add<ControllerExceptionFilter>();
        }).AddAuthorization().AddCors().AddApiExplorer();

        if (!Program.LimitMode)
        {
            //builder.Services.AddRazorPages();
            MvcBuilder.AddRazorPages();
        }
    }

    public static void AddHealth(IServiceCollection services, HealthCheckService health)
    {
        services.AddSingleton(health);
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("Database")
            .AddCheck<EmailHealthCheck>("Email");
    }

    public static void AddIdentity(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = string.Join("", UsernameValidationAttribute.AllowedCharacters);
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
        });

        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.Name = "DevSpace.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/error?type=AccessDenied";
        });
    }

    public static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.DocInclusionPredicate((_, description) =>
            {
                ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)description.ActionDescriptor;

                if (actionDescriptor.ControllerTypeInfo.GetCustomAttribute<ShowInSwaggerAttribute>() != null
                       || actionDescriptor.MethodInfo.GetCustomAttribute<ShowInSwaggerAttribute>() != null)
                {
                    return true;
                }

                return false;
            });
            OpenApiInfo Info = new OpenApiInfo
            {
                Title = "Dev Space API",
                Version = "v1",
                Description = "API server for this Dev Space instance.",
                //TermsOfService = new Uri("https://fluxpoint.dev/terms"),
            };
            if (!string.IsNullOrEmpty(_Data.Config.Instance.Email))
            {
                Info.Contact = new OpenApiContact
                {
                    Email = _Data.Config.Instance.Email,
                    Name = "Support Email"
                };
            }
            c.SwaggerDoc("client", Info);

            c.AddServer(new OpenApiServer
            {
                Url = string.IsNullOrEmpty(_Data.Config.Instance.PublicDomain) ? "http://localhost" : $"https://{_Data.Config.Instance.PublicDomain}"
            });
            c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: false);
            c.AddSecurityDefinition("key", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Description = "Create an API Key in your Team page.",
                In = ParameterLocation.Header,
                Name = "Authorization"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "key" }
                    },
                    new string[] {  }
                }
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.OperationFilter<SwaggerCheckAuthFilter>();
            c.SchemaFilter<SwaggerFieldSchemaFilter>();
        });
    }



    public static void AddProviders(IServiceCollection services)
    {
        AuthenticationBuilder Auth = services.AddAuthentication();
        if (_Data.Config.Providers.Fluxpoint.IsConfigured())
        {
            Logger.LogMessage("Fluxpoint oauth login enabled", LogSeverity.Info);
            //Auth.AddOAuth("fluxpoint", opt =>
            //{
            //    opt.AuthorizationEndpoint = "https://auth.fluxpoint.dev/application/o/authorize/";
            //    opt.CallbackPath = new PathString("/auth/login/fluxpoint");
            //    opt.ClientId = _Data.Config.Providers.Fluxpoint.ClientId;
            //    opt.ClientSecret = _Data.Config.Providers.Fluxpoint.ClientSecret;
            //    opt.Scope.Add("profile");
            //    opt.Scope.Add("email");
            //    opt.SignInScheme = IdentityConstants.ExternalScheme;
            //    opt.TokenEndpoint = "https://auth.fluxpoint.dev/application/o/token/";
            //    opt.UserInformationEndpoint = "https://auth.fluxpoint.dev/application/o/userinfo/";
            //    opt.Events.OnCreatingTicket += async context =>
            //    {
            //        if (context.AccessToken is { })
            //        {
            //            Console.WriteLine("Access: " + context.AccessToken);
            //            foreach (var i in context.Principal.Claims)
            //            {
            //                Console.WriteLine($"Claim: {i.Type} - {i.Value}");
            //            }
            //        }
            //    };
            //    opt.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "pk");
            //    opt.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
            //    opt.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            //});
            Auth.AddOpenIdConnect("fluxpoint", "Fluxpoint", opt =>
            {
                opt.CallbackPath = new PathString("/auth/login/fluxpoint");
                opt.ClientId = _Data.Config.Providers.Fluxpoint.ClientId;
                opt.ClientSecret = _Data.Config.Providers.Fluxpoint.ClientSecret;
                opt.Scope.Add("profile");
                opt.Scope.Add("email");
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "pk");
                opt.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
                opt.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.Authority = "https://auth.fluxpoint.dev/application/o/dev-space/";
            });
            //Auth.AddTwitter("fluxpoint", opt =>
            //{
            //    opt.ConsumerKey = _Data.Config.Providers.Twitter.ConsumerKey;
            //    opt.ConsumerSecret = _Data.Config.Providers.Twitter.ConsumerSecret;
            //    opt.SignInScheme = IdentityConstants.ExternalScheme;
            //    opt.RetrieveUserDetails = true;
            //    opt.CallbackPath = new PathString("/auth/login/twitter");
            //});
        }
        if (_Data.Config.Providers.Apple.IsConfigured())
        {
            Logger.LogMessage("Apple oauth login enabled", LogSeverity.Info);
            Auth.AddApple("apple", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Apple.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.Apple.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/apple");
                opt.Scope.Add("name");
                opt.Scope.Add("email");

            });
        }
        if (_Data.Config.Providers.Discord.IsConfigured())
        {
            Logger.LogMessage("Discord oauth login enabled", LogSeverity.Info);
            Auth.AddDiscord("discord", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Discord.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.Discord.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/discord");
                opt.Scope.Add("email");
                opt.Prompt = "none";
            });
        }
        if (_Data.Config.Providers.Google.IsConfigured())
        {
            Logger.LogMessage("Google oauth login enabled", LogSeverity.Info);
            Auth.AddGoogle("google", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Google.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.Google.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/google");
                opt.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
            });
        }
        if (_Data.Config.Providers.GitHub.IsConfigured())
        {
            Logger.LogMessage("GitHub oauth login enabled", LogSeverity.Info);
            Auth.AddGitHub("github", opt =>
            {
                opt.ClientId = _Data.Config.Providers.GitHub.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.GitHub.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/github");
                opt.Scope.Add("user:email");
                opt.UserEmailsEndpoint = GitHubAuthenticationDefaults.UserEmailsEndpoint;
                opt.Events.OnCreatingTicket += async context =>
                {
                    if (context.AccessToken is { })
                    {
                        if (context.Identity == null)
                            throw new Exception("GitHub identity failed.");

                        Claim? EmailClaim = context.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                        if (EmailClaim != null)
                            context.Identity.RemoveClaim(EmailClaim);

                        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GitHubAuthenticationDefaults.UserEmailsEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        using HttpResponseMessage response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException("An error occurred while retrieving the email address associated to the user profile.");
                        }

                        using JsonDocument payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                        context.Identity.AddClaim(new Claim(ClaimTypes.Email, (from address in payload.RootElement.EnumerateArray()
                                                                               where address.GetProperty("primary").GetBoolean()
                                                                               select address.GetString("email")).FirstOrDefault(), ClaimValueTypes.String, context.Options.ClaimsIssuer));


                        //Console.WriteLine("Access: " + context.AccessToken);
                        //context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
                    }
                };
            });
        }
        if (_Data.Config.Providers.GitLab.IsConfigured())
        {
            Logger.LogMessage("GitLab oauth login enabled", LogSeverity.Info);
            Auth.AddGitLab("gitlab", opt =>
            {
                opt.ClientId = _Data.Config.Providers.GitLab.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.GitLab.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/gitlab");
                opt.Scope.Add("email");

            });
        }
        if (_Data.Config.Providers.Slack.IsConfigured())
        {
            Logger.LogMessage("Slack oauth login enabled", LogSeverity.Info);
            Auth.AddSlack("slack", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Slack.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.Slack.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.CallbackPath = new PathString("/auth/login/slack");
                opt.Scope.Add("email");
                opt.Events.OnCreatingTicket = async context =>
                {

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, SlackAuthenticationDefaults.UserInformationEndpoint))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Add("Authorization", "Bearer " + context.AccessToken);
                        using HttpResponseMessage response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                throw new HttpRequestException("An error occurred while retrieving the user profile.");
                            }

                            if (context.Identity == null)
                                throw new Exception("Slack identity failed.");

                            using (JsonDocument payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(context.HttpContext.RequestAborted)))
                            {
                                Claim? IdClaim = context.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                                if (IdClaim != null)
                                    context.Identity.RemoveClaim(IdClaim);

                                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, payload.RootElement.GetProperty("user").GetString("id") ?? "", ClaimValueTypes.String, context.Options.ClaimsIssuer));
                                context.Identity.AddClaim(new Claim(ClaimTypes.Name, payload.RootElement.GetProperty("user").GetString("name") ?? "", ClaimValueTypes.String, context.Options.ClaimsIssuer));
                                context.Identity.AddClaim(new Claim(ClaimTypes.Email, payload.RootElement.GetProperty("user").GetString("email") ?? "", ClaimValueTypes.String, context.Options.ClaimsIssuer));
                                context.Identity.AddClaim(new Claim("team", payload.RootElement.GetProperty("team").GetString("id") ?? "", ClaimValueTypes.String, context.Options.ClaimsIssuer));

                            }
                        }
                    }



                    //foreach (var i in context.Principal.Claims)
                    //{
                    //    Console.WriteLine($"Claim: {i.Type} - {i.Value}");
                    //}
                    //Console.WriteLine("Access: " + context.AccessToken);
                    //if (context.AccessToken is { })
                    //{
                    //}
                };
            });
        }
        if (_Data.Config.Providers.Twitter.IsConfigured())
        {
            Logger.LogMessage("Twitter oauth login enabled", LogSeverity.Info);
            Auth.AddTwitter("twitter", opt =>
            {
                opt.ConsumerKey = _Data.Config.Providers.Twitter.ConsumerKey;
                opt.ConsumerSecret = _Data.Config.Providers.Twitter.ConsumerSecret;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.RetrieveUserDetails = true;
                opt.CallbackPath = new PathString("/auth/login/twitter");
            });
        }
        if (_Data.Config.Providers.Microsoft.IsConfigured())
        {
            Logger.LogMessage("Microsoft oauth login enabled", LogSeverity.Info);
            Auth.AddMicrosoftAccount("microsoft", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Microsoft.ClientId!;
                opt.ClientSecret = _Data.Config.Providers.Microsoft.ClientSecret!;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                //opt.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                //opt.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
                opt.CallbackPath = new PathString("/auth/login/microsoft");
            });
        }

    }

    public static void AddFido2(IServiceCollection services)
    {
        HashSet<string> Origins = new HashSet<string>();
        if (Program.IsDevMode)
        {
            Origins.Add("https://localhost:5149");
        }
        else
        {
            if (!string.IsNullOrEmpty(_Data.Config.Instance.PublicDomain))
                Origins.Add("https://" + _Data.Config.Instance.PublicDomain);
        }

        services.AddSingleton(new Fido2Service());
    }

    public static void AddMongoDb(IServiceCollection services)
    {
        // Add MongoDB Auth
        services.AddIdentity<AuthUser, UserRole>()
            .AddSignInManager()
            .AddTokenProvider(TokenOptions.DefaultProvider, typeof(DataProtectorTokenProvider<AuthUser>))
            .AddTokenProvider(TokenOptions.DefaultEmailProvider, typeof(EmailTokenProvider<AuthUser>))
            .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(AuthenticatorTokenProvider<AuthUser>))
        .AddMongoDbStores2<CustomMongoDbContext, AuthUser, UserRole, ObjectId>(new CustomMongoDbContext(_DB.Run));

    }

    public static void AddEmail(IServiceCollection services, HealthCheckService health)
    {
        services.AddTransient<IEmailSender<AuthUser>, EmailSender>()
            .AddSingleton(new EmailService(health));
    }


    public static IdentityBuilder AddMongoDbStores2<TContext, TUser, TRole, TKey>(this IdentityBuilder builder, IMongoDbContext mongoDbContext) where TContext : IMongoDbContext where TUser : MongoIdentityUser<TKey>, new() where TRole : MongoIdentityRole<TKey>, new() where TKey : IEquatable<TKey>
    {
        if (mongoDbContext == null)
        {
            throw new ArgumentNullException("mongoDbContext");
        }

        builder.Services.TryAddSingleton(mongoDbContext);
        builder.Services.TryAddSingleton((IMongoRepository)new MongoRepository(mongoDbContext));
        builder.Services.TryAddScoped((Func<IServiceProvider, IUserStore<TUser>>)((IServiceProvider provider) => new MongoUserStore<TUser, TRole, IMongoDbContext, TKey>(mongoDbContext)));
        builder.Services.TryAddScoped((Func<IServiceProvider, IRoleStore<TRole>>)((IServiceProvider provider) => new MongoRoleStore<TRole, IMongoDbContext, TKey>(mongoDbContext)));
        return builder;
    }
}