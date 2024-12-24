using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Auth;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Authentication;
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
        Fido2Service fido2Service, IMemoryCache cache) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
        //User.SetToken(new IdentityUserToken<Guid>(), "test");
    }

    public IMemoryCache Cache { get; private set; }

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
        


        return Challenge(properties, provider);
    }

    [HttpGet("/auth/external/callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");
        if (!Program.IsDevMode)
            return BadRequest("Not ready yet.");

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

        AuthUser? AuthUser = await _userManager.FindByEmailAsync(Email);
        if (AuthUser != null)
        {
            IdentityResult Result = await _userManager.AddLoginAsync(AuthUser, info);
            await _signInManager.SignInAsync(AuthUser, true);
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
}
