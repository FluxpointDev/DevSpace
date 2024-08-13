using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DevSpaceWeb.Controllers;

public class LoginForm
{
    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Email length can't be less than 8.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Email length can't be less than 8.")]
    public string Password { get; set; }

    public string RequestVerificationToken { get; set; }
}

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
    public async Task<IActionResult> Login([FromForm] string Email = "", [FromForm] string Password = "", [FromForm] string RequestVerificationToken = "", [FromForm] bool RememberMe = false)
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            return BadRequest("Could not login");


        var AuthUser = await _userManager.FindByEmailAsync(Email);
        if (AuthUser == null)
            return BadRequest("Failed to login");

        var Result = await _signInManager.PasswordSignInAsync(AuthUser, Password, RememberMe, false);
        if (!Result.Succeeded)
        {
            return BadRequest("Failed to login");
        }

        AuthUser.Auth.LoginAt = DateTimeOffset.UtcNow;
        _signInManager.UserManager.UpdateAsync(AuthUser);

        return Redirect("/");
    }

    [Route("/auth/external"), HttpGet, HttpPost]
    public async Task<IActionResult> LoginExternalAsync([FromForm] string provider = "")
    {
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
    public async Task Logout()
    {
        if (!User.Identity.IsAuthenticated)
            return;

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);

        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.Logout);

        await HttpContext.SignOutAsync("Cookies", new AuthenticationProperties
        {
            RedirectUri = "/"
        });
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