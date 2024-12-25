using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Auth;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using OpenIddict.Core;
using System.Security.Claims;

namespace DevSpaceWeb.Controllers.Auth;

public class AuthLoginController : AuthControllerContext
{
    public AuthLoginController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        GoogleHandler auth,
        Fido2Service fido2Service, IMemoryCache cache, OpenIddictClientService openclient, IOptionsMonitor<OpenIddictClientOptions> reg) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
        OpenClient = openclient;
        //User.SetToken(new IdentityUserToken<Guid>(), "test");

    }

    public IMemoryCache Cache { get; private set; }

    public OpenIddictClientService OpenClient { get; set; }

    [HttpGet("/testclient")]
    public async Task Test()
    {
        var Client = await OpenClient.GetClientRegistrationByProviderNameAsync("github");
        try
        {
            Console.WriteLine("Client");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Client, Newtonsoft.Json.Formatting.Indented));
            Client.ClientId = "test";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    // Login User
    [HttpPost("/auth/login")]
    public async Task<IActionResult> Login([FromForm] string email = "", [FromForm] string password = "", [FromHeader] string RequestId = "", [FromForm] bool rememberMe = false)
    {
        if (string.IsNullOrEmpty(RequestId) || !Cache.TryGetValue("login-" + RequestId, out UserSessionJson SessionJson))
            return BadRequest("Request is invalid or expired");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return BadRequest("Invalid email or password.");

        if (SessionJson.Email != email)
            return BadRequest("Request is invalid or expired");

        AuthUser? AuthUser = await _userManager.FindByEmailAsync(email);
        if (AuthUser == null)
        {
            if (Program.Cache.TryGetValue(Utils.GetStringSha256Hash(Utils.GetUserIpAddress(Request.HttpContext)), out LoginTryCount? loginTry))
            {
                if (loginTry.Count == 9)
                    return StatusCode(429);

                loginTry.Count += 1;
            }
            else
            {
                Program.Cache.Set(Utils.GetStringSha256Hash(Utils.GetUserIpAddress(Request.HttpContext)), new LoginTryCount(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)));
            }
            return BadRequest("Invalid email or password.");
        }

        Microsoft.AspNetCore.Identity.SignInResult Result = await _signInManager.PasswordSignInAsync(AuthUser, password, rememberMe, false);
        if (!Result.Succeeded)
        {
            if (Program.Cache.TryGetValue(Utils.GetStringSha256Hash(Utils.GetUserIpAddress(Request.HttpContext)), out LoginTryCount? loginTry))
            {
                if (loginTry.Count == 9)
                    return StatusCode(429);

                loginTry.Count += 1;
            }
            else
            {
                Program.Cache.Set(Utils.GetStringSha256Hash(Utils.GetUserIpAddress(Request.HttpContext)), new LoginTryCount(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)));
            }
            return BadRequest("Invalid email or password.");
        }
        string SessionId = Request.Cookies["DevSpace.SessionId"];
        if (string.IsNullOrEmpty(SessionId))
        {
            SessionId = Guid.NewGuid().ToString();
            Response.Cookies.Append("DevSpace.SessionId", SessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = rememberMe ? DateTimeOffset.Now.AddDays(30) : null
            });
        }

        if (!AuthUser.Account.Sessions.TryGetValue(SessionId, out UserSession Session))
            AuthUser.Account.Sessions.Add(SessionId, UserSession.Create(SessionJson));
        else
            Session.LastLoginAt = DateTime.UtcNow;

        AuthUser.Account.LoginAt = DateTime.UtcNow;
        await _signInManager.UserManager.UpdateAsync(AuthUser);

        return Ok();
    }

    [HttpPost("/auth/login/external")]
    public async Task<IActionResult> LoginExternalAsync([FromForm] string provider = "", [FromHeader] string RequestId = "", [FromForm] bool rememberMe = false)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");
        if (!Program.IsDevMode)
            return BadRequest("Not ready yet.");
        string redirectUrl = "/auth/external/callback";

        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        foreach(var i in properties.GetTokens())
        {
            Console.WriteLine($"TK: {i.Name}-{i.Value}");
        }


        return Challenge(properties, provider);
    }

    [Route("/auth/external/callback"), HttpGet, HttpPost]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");
        if (!Program.IsDevMode)
            return BadRequest("Not ready yet.");

        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            Console.WriteLine("External login");
            return RedirectToAction("/login");
        }
        
        string? Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        string? Name = info.Principal.FindFirstValue(ClaimTypes.Name);

        AuthUser? AuthUser = await _userManager.FindByEmailAsync(Email);
        if (AuthUser != null)
        {
            IdentityResult Result = await _userManager.AddLoginAsync(AuthUser, info);

            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            foreach (var item in info.AuthenticationProperties.Items)
            {
                Console.WriteLine(item.Key + " - " + item.Value);
            }




            return Redirect(returnUrl);
        }

        //Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //if (signInResult.Succeeded)
        //{
        //    return Redirect("/");
        //}
        //if (signInResult.IsLockedOut)
        //{
        //    //return RedirectToAction(nameof(ForgotPassword));
        //}
        //else
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    ViewData["Provider"] = info.LoginProvider;
        //    string email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    //return View("ExternalLogin", new ExternalLoginModel { Email = email });
        //}

        return Ok();
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        if (!User.Identity.IsAuthenticated)
            return Redirect("/");

        if (!Program.IsPreviewMode)
        {
            AuthUser? AuthUser = await _userManager.GetUserAsync(User);
            if (AuthUser != null)
            {
                string SessionId = Request.Cookies["DevSpace.SessionId"];
                if (AuthUser.Account.Sessions.TryGetValue(SessionId, out UserSession Session))
                {
                    Session.AuthorizedIps.Clear();
                    await _userManager.UpdateAsync(AuthUser);
                }
                _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.Logout);
            }
        }

        await _signInManager.SignOutAsync();
        return Redirect("/");
    }

    [HttpGet("/callback/login/{provider}"), HttpPost("/callback/login/{provider}"), IgnoreAntiforgeryToken]
    public async Task<ActionResult> LogInCallback()
    {
        // Retrieve the authorization data validated by OpenIddict as part of the callback handling.
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        // Important: if the remote server doesn't support OpenID Connect and doesn't expose a userinfo endpoint,
        // result.Principal.Identity will represent an unauthenticated identity and won't contain any user claim.
        //
        // Such identities cannot be used as-is to build an authentication cookie in ASP.NET Core (as the
        // antiforgery stack requires at least a name claim to bind CSRF cookies to the user's identity) but
        // the access/refresh tokens can be retrieved using result.Properties.GetTokens() to make API calls.
        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        // Build an identity based on the external claims and that will be used to create the authentication cookie.
        var identity = new ClaimsIdentity(authenticationType: "ExternalLogin");
        // By default, OpenIddict will automatically try to map the email/name and name identifier claims from
        // their standard OpenID Connect or provider-specific equivalent, if available. If needed, additional
        // claims can be resolved from the external identity and copied to the final authentication cookie.
        identity.SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
                .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
                .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));

        // Preserve the registration identifier to be able to resolve it later.
        identity.SetClaim(OpenIddictConstants.Claims.Private.RegistrationId, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId));

        // Build the authentication properties based on the properties that were added when the challenge was triggered.
        var properties = new AuthenticationProperties(result.Properties.Items)
        {
            RedirectUri = result.Properties.RedirectUri ?? "/"
        };

        try
        {

            // If needed, the tokens returned by the authorization server can be stored in the authentication cookie.
            //
            // To make cookies less heavy, tokens that are not used are filtered out before creating the cookie.
            properties.StoreTokens(result.Properties.GetTokens().Where(token => token.Name is
            // Preserve the access, identity and refresh tokens returned in the token response, if available.
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
            OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or
            OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken));

        //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        


            ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            foreach (AuthenticationToken i in info.AuthenticationTokens)
            {
                Logger.LogMessage($"Token: {i.Name} - {i.Value}", LogSeverity.Info);
            }

            string? Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            string? Name = info.Principal.FindFirstValue(ClaimTypes.Name);
            Console.WriteLine($"Get: {Email} - {Name}");
            AuthUser? AuthUser = await _userManager.FindByEmailAsync(Email);
            if (AuthUser != null)
            {
                IdentityResult Result = await _userManager.AddLoginAsync(AuthUser, info);
                await _signInManager.SignInAsync(AuthUser, true);
                Console.WriteLine("Redirect: " + properties.RedirectUri);
                return Redirect("/");
            }
            else
            {
                Console.WriteLine("No User");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return BadRequest();

        // Ask the default sign-in handler to return a new cookie and redirect the
        // user agent to the return URL stored in the authentication properties.
        //
        // For scenarios where the default sign-in handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        //return SignIn(new ClaimsPrincipal(identity), properties);
    }
}
