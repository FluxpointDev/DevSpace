﻿using AspNetCore.Identity.MongoDbCore;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using DevSpaceWeb.Fido2;
using DevSpaceWeb.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Radzen;
using System.Reflection;

namespace DevSpaceWeb.Services;

public static class ServiceBuilder
{
    public static void Build(WebApplicationBuilder builder, IServiceCollection services)
    {
        // Add HTTP access
        services.AddHttpContextAccessor();
        services.AddScoped<HttpContextAccessor>();
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
        AddFido2(services);
        AddIdentity(services);
        AddMongoDb(services);
        AddEmail(services);
        AddProviders(services);

        if (_Data.Config.Instance.Features.SwaggerEnabled)
            AddSwagger(services);

    }

    public static void AddIdentity(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = AccountRegisterModel.AllowedCharacters;
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
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });
    }

    public static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.DocInclusionPredicate((_, description) =>
            {
                ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)description.ActionDescriptor;

                return actionDescriptor.ControllerTypeInfo.GetCustomAttributes<ShowInSwaggerAttribute>().Any()
                       || actionDescriptor.MethodInfo.GetCustomAttributes<ShowInSwaggerAttribute>().Any();

                //or any other visibility strategy...
            });
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dev Space API",
                Version = "v1",
                Description = "API server for this Dev Space instance.",
                //TermsOfService = new Uri("https://fluxpoint.dev/terms"),
                //Contact = new OpenApiContact
                //{
                //    Email = "support@fluxpoint.dev",
                //    Name = "Fluxpoint Support",
                //    Url = new Uri("https://discord.gg/fluxpoint")
                //}
            });
            //c.AddServer(new OpenApiServer
            //{
            //    Url = "https://api.fluxpoint.dev"
            //});
            //c.ExampleFilters();
            //c.EnableAnnotations(enableAnnotationsForInheritance: false, enableAnnotationsForPolymorphism: false);
            c.AddSecurityDefinition("key", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Description = "Create an API Key in your Team page.",
                In = ParameterLocation.Header,
                Name = "Authorization"
            });
            //            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "key" },

            //        },
            //        new string[] {  }
            //                }
            //            });

            c.OperationFilter<SwaggerCheckAuthFilter>();
        });
    }

    public static void AddProviders(IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseMongoDb(config =>
                {
                    config.UseDatabase(_DB.Run);
                });
            })
            .AddClient(options =>
            {
                // Note: this sample only uses the authorization code flow,
                // but you can enable the other flows if necessary.
                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials used to protect
                // sensitive data like the state tokens produced by OpenIddict.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableRedirectionEndpointPassthrough();

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();

                // Register the Web providers integrations.
                //
                // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                // URI per provider, unless all the registered providers support returning a special "iss"
                // parameter containing their URL as part of authorization responses. For more information,
                // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                options.UseWebProviders()
                       .AddGitHub(options =>
                       {
                           options.SetClientId(_Data.Config.Providers.GitHub.ClientId)
                                  .SetClientSecret(_Data.Config.Providers.GitHub.ClientSecret)
                                  .SetProviderName("github")
                                  .AddScopes("user:email", "read:user")
                                  .SetRedirectUri("callback/login/github");
                       });
            });

        Microsoft.AspNetCore.Authentication.AuthenticationBuilder Auth = services.AddAuthentication();
        if (!string.IsNullOrEmpty(_Data.Config.Providers.Google.ClientId) && !string.IsNullOrEmpty(_Data.Config.Providers.Google.ClientSecret))
        {
            Auth.AddGoogle("google", opt =>
                {
                    opt.ClientId = _Data.Config.Providers.Google.ClientId;
                    opt.ClientSecret = _Data.Config.Providers.Google.ClientSecret;
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                });
        }
        if (!string.IsNullOrEmpty(_Data.Config.Providers.Discord.ClientId) && !string.IsNullOrEmpty(_Data.Config.Providers.Discord.ClientSecret))
        {
            Auth.AddDiscord("discord", opt =>
            {
                opt.ClientId = _Data.Config.Providers.Discord.ClientId;
                opt.ClientSecret = _Data.Config.Providers.Discord.ClientSecret;
                opt.SignInScheme = IdentityConstants.ExternalScheme;
                opt.Scope.Add("email");
                opt.Prompt = "none";
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

    public static void AddEmail(IServiceCollection services)
    {
        services.AddTransient<IEmailSender<AuthUser>, EmailSender>()
            .AddSingleton(new EmailService());
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