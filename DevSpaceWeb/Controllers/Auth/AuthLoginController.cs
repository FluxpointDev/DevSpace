using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace DevSpaceWeb.Controllers.Auth;

public class AuthLoginController : AuthControllerContext
{
    public AuthLoginController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service, IDistributedCache cache) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
        //User.SetToken(new IdentityUserToken<Guid>(), "test");
    }

    public IDistributedCache Cache { get; private set; }

    // Login User
    [HttpPost("/auth/login")]
    public async Task<IActionResult> Login([FromForm] string email = "", [FromForm] string password = "", [FromHeader] string requestId = "", [FromForm] bool rememberMe = false, [FromForm] bool isMobile = false, [FromForm] int browser = 0, [FromForm] string country = "")
    {
        if (string.IsNullOrEmpty(requestId))
            return BadRequest("Request id is invalid");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return BadRequest("Invalid email or password.");

        string Data = Cache.GetString("login-" + requestId);

        if (string.IsNullOrEmpty(Data) || Data != email)
            return BadRequest("Invalid email or password.");

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

        if (!AuthUser.Account.Sessions.ContainsKey(SessionId))
        {
            string BrowserName = "";
            switch ((SessionBrowserType)browser)
            {
                case SessionBrowserType.Chrome:
                    break;
            }
            AuthUser.Account.Sessions.Add(SessionId, new UserSession
            {
                CreatedAt = DateTime.UtcNow,
                BrowserType = (SessionBrowserType)browser,
                IsMobile = isMobile,
                Country = country,
                Name = Utils.GetBrowserName((SessionBrowserType)browser)
            });
        }

        AuthUser.Account.LoginAt = DateTime.UtcNow;
        await _signInManager.UserManager.UpdateAsync(AuthUser);

        return Ok();
    }

    [Route("/auth/external"), HttpGet, HttpPost]
    public async Task<IActionResult> LoginExternalAsync([FromForm] string provider = "")
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        string returnUrl = "/";
        string redirectUrl = "/auth/external/callback";
        Logger.LogMessage(redirectUrl, LogSeverity.Debug);
        Microsoft.AspNetCore.Authentication.AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);

        return Ok();
    }

    [HttpGet("/auth/external/callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        foreach (Microsoft.AspNetCore.Authentication.AuthenticationToken i in info.AuthenticationTokens)
        {
            Logger.LogMessage($"Token: {i.Name} - {i.Value}", LogSeverity.Debug);
        }

        string? Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        string? Name = info.Principal.FindFirstValue(ClaimTypes.Name);
        string? NameId = info.Principal.FindFirstValue(ClaimTypes.GivenName);

        return Ok();


        Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return Redirect("/");
        }
        if (signInResult.IsLockedOut)
        {
            //return RedirectToAction(nameof(ForgotPassword));
        }
        else
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Provider"] = info.LoginProvider;
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //return View("ExternalLogin", new ExternalLoginModel { Email = email });
        }

        return Ok();
    }


    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        if (!User.Identity.IsAuthenticated)
            return Redirect("/");

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);

        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.Logout);

        await _signInManager.SignOutAsync();
        Response.Cookies.Delete("DevSpace.SessionId");
        return Redirect("/");
    }
}


//public class RevalidatingIdentityAuthenticationStateProvider<TUser>
//        : RevalidatingServerAuthenticationStateProvider where TUser : class
//{
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly IdentityOptions _options;

//    public RevalidatingIdentityAuthenticationStateProvider(
//        ILoggerFactory loggerFactory,
//        IServiceScopeFactory scopeFactory,
//        IOptions<IdentityOptions> optionsAccessor)
//        : base(loggerFactory)
//    {
//        _scopeFactory = scopeFactory;
//        _options = optionsAccessor.Value;
//    }

//    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

//    protected override async Task<bool> ValidateAuthenticationStateAsync(
//        AuthenticationState authenticationState, CancellationToken cancellationToken)
//    {
//        // Get the user manager from a new scope to ensure it fetches fresh data
//        var scope = _scopeFactory.CreateScope();
//        try
//        {
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
//            return await ValidateSecurityStampAsync(userManager, authenticationState.User);
//        }
//        finally
//        {
//            if (scope is IAsyncDisposable asyncDisposable)
//            {
//                await asyncDisposable.DisposeAsync();
//            }
//            else
//            {
//                scope.Dispose();
//            }
//        }
//    }

//    private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
//    {
//        var user = await userManager.GetUserAsync(principal);
//        if (user == null)
//        {
//            return false;
//        }
//        else if (!userManager.SupportsUserSecurityStamp)
//        {
//            return true;
//        }
//        else
//        {
//            var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
//            var userStamp = await userManager.GetSecurityStampAsync(user);
//            return principalStamp == userStamp;
//        }
//    }
//}