using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace DevSpaceWeb.Controllers;

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
    public async Task<IActionResult> Login([FromForm] string email = "", [FromForm] string password = "", [FromHeader] string requestId = "", [FromForm] bool rememberMe = false)
    {
        if (string.IsNullOrEmpty(requestId))
            return BadRequest("Request id is invalid");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return BadRequest("Invalid email or password.");

        string Data = Cache.GetString("login-" + requestId);

        if (string.IsNullOrEmpty(Data) || Data != email)
            return BadRequest("Request id is invalid");

        var AuthUser = await _userManager.FindByEmailAsync(email);
        if (AuthUser == null)
            return BadRequest("Invalid email or password.");

        var Result = await _signInManager.PasswordSignInAsync(AuthUser, password, rememberMe, false);
        if (!Result.Succeeded)
            return BadRequest("Invalid email or password.");

        AuthUser.Auth.LoginAt = DateTimeOffset.UtcNow;
        await _signInManager.UserManager.UpdateAsync(AuthUser);
        return Ok();
    }

    [Route("/auth/external"), HttpGet, HttpPost]
    public async Task<IActionResult> LoginExternalAsync([FromForm] string provider = "")
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        string returnUrl = "/";
        var redirectUrl = "/auth/external/callback";
        Console.WriteLine(redirectUrl);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);

        return Ok();
    }

    [HttpGet("/auth/external/callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        foreach (var i in info.AuthenticationTokens)
        {
            Console.WriteLine($"Token: {i.Name} - {i.Value}");
        }

        var Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var Name = info.Principal.FindFirstValue(ClaimTypes.Name);
        var NameId = info.Principal.FindFirstValue(ClaimTypes.GivenName);

        return Ok();


        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
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
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
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