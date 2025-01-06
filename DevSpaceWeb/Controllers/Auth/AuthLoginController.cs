using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
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
        GoogleHandler auth,
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

        if (!_Data.Config.Auth.AllowInternalLogin)
            return Unauthorized("Internal login has been disabled on this instance.");

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
    public async Task<IActionResult> LoginExternalAsync([FromForm] string provider = "", [FromForm] string requestId = "", [FromForm] bool rememberMe = false)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        bool AlreadyAuthed = false;
        if (Request.HttpContext.User.Identity != null && Request.HttpContext.User.Identity.IsAuthenticated)
            AlreadyAuthed = true;

        if (!_Data.Config.Auth.AllowExternalLogin)
            return Unauthorized("External login has been disabled on this instance.");

        if (!AlreadyAuthed && (string.IsNullOrEmpty(requestId) || !Cache.TryGetValue("login-" + requestId, out UserSessionJson SessionJson)))
            return BadRequest("Request is invalid or expired");

        string redirectUrl = "/auth/external/callback";

        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items.Add("RememberMe", rememberMe.ToString());
        properties.Items.Add("RequestId", requestId);
        //foreach(var i in properties.GetTokens())
        //{
        //    Console.WriteLine($"TK: {i.Name}-{i.Value}");
        //}


        return Challenge(properties, provider);
    }

    [Route("/auth/external/callback"), HttpGet, HttpPost]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (!_Data.Config.Auth.AllowExternalLogin)
            return Unauthorized("External login has been disabled on this instance.");

        bool AlreadyAuthed = false;
        if (Request.HttpContext.User.Identity != null && Request.HttpContext.User.Identity.IsAuthenticated)
            AlreadyAuthed = true;

        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = "/";

        ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            if (AlreadyAuthed)
                return Redirect(returnUrl);

            return Redirect("/login");
        }

        //foreach (var item in info.AuthenticationProperties.Items)
        //{
        //    Console.WriteLine("Prop: " + item.Key + " - " + item.Value);
        //}
        //foreach (var i in info.AuthenticationTokens)
        //{
        //    Console.WriteLine($"Token: {i.Name} - {i.Value}");
        //}
        //foreach (var p in info.Principal.Claims)
        //{
        //    Console.WriteLine($"Claim: {p.Subject}/{p.Type} - {p.Value}/{p.ValueType}");
        //}


        string? Email = info.Principal.FindFirstValue(ClaimTypes.Email);
        string? Name = info.Principal.FindFirstValue(ClaimTypes.Name);
        string? Id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        //Console.WriteLine($"Got: {Email} - {Name}");
        if (string.IsNullOrEmpty(Email))
            return BadRequest("Email is missing from provider " + info.ProviderDisplayName);
        if (string.IsNullOrEmpty(Id))
            return BadRequest("User Id is missing from provider " + info.ProviderDisplayName);

        bool RememberMe = false;
        string? RequestId = null;
        if (info.AuthenticationProperties.Items.TryGetValue("RememberMe", out string val))
            bool.TryParse(val, out RememberMe);
        info.AuthenticationProperties.Items.TryGetValue("RequestId", out RequestId);
        UserSessionJson? SessionJson = null;
        if (!AlreadyAuthed && (string.IsNullOrEmpty(RequestId) || !Cache.TryGetValue("login-" + RequestId, out SessionJson)))
            return BadRequest("Request is invalid or expired");

        AuthUser? AuthUser = await _userManager.FindByEmailAsync(Email);
        if (AuthUser == null)
            return Redirect("/register?email=" + Email);

        if (AlreadyAuthed && !Email.Equals(Request.HttpContext.User.FindFirstValue(ClaimTypes.Email), StringComparison.OrdinalIgnoreCase))
            return BadRequest("Email does not match the logged in user.");

        if (!AuthUser.Logins.Any(x => x.LoginProvider == info.LoginProvider))
        {
            if (AlreadyAuthed)
                await _userManager.AddLoginAsync(AuthUser, info);
            else
                return Redirect("/login?link&email=" + Email);
        }

        if (!AlreadyAuthed)
        {
            string SessionId = Request.Cookies["DevSpace.SessionId"];
            if (string.IsNullOrEmpty(SessionId))
            {
                SessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("DevSpace.SessionId", SessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = RememberMe ? DateTimeOffset.Now.AddDays(30) : null
                });
            }

            if (!AuthUser.Account.Sessions.TryGetValue(SessionId, out UserSession Session))
                AuthUser.Account.Sessions.Add(SessionId, UserSession.Create(SessionJson));
            else
                Session.LastLoginAt = DateTime.UtcNow;

            AuthUser.Account.LoginAt = DateTime.UtcNow;
            await _signInManager.UserManager.UpdateAsync(AuthUser);

            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, RememberMe, true);
        }
        else
            returnUrl = "/user/account";


        return Redirect(returnUrl);
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
