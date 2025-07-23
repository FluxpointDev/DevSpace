using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using DevSpaceWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace DevSpaceWeb.Controllers.Auth;

public class AuthAccountController : AuthControllerContext
{
    public AuthAccountController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service, IMemoryCache cache, EmailService email) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
        Email = email;
    }

    private readonly IMemoryCache Cache;
    private readonly EmailService Email;

    // Download Recovery Code
    [HttpGet("/auth/account/downloadRecoveryCode")]
    public async Task<IActionResult> RecoveryCode([FromQuery] string token = "")
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Redirect("/login");

        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(token))
            return BadRequest("Invalid token");

        string? CodeString = Cache.Get<string>(token);
        if (string.IsNullOrEmpty(CodeString))
            return BadRequest("Invalid token or expired");

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);

        if (AuthUser == null)
            return BadRequest("Invalid user account");

        if (string.IsNullOrEmpty(AuthUser.Mfa.RecoveryCode))
            return BadRequest("You do not have any recovery codes");


        string data = CodeString;
        MemoryStream content = new MemoryStream(Encoding.ASCII.GetBytes(data));
        string contentType = "APPLICATION/octet-stream";
        string fileName = $"{AuthUser.Email!.Replace(".", "_")} Codes.txt";
        return File(content, contentType, fileName);
    }

    [HttpGet("/auth/account/confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token = "")
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (!Email.CanSendEmail())
            return BadRequest("Email service has not been setup.");

        if (string.IsNullOrEmpty(token))
            return BadRequest("Invalid token");

        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Redirect("/login?token=" + token);

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);
        if (AuthUser == null)
            return BadRequest("Invalid user account");

        if (AuthUser.EmailConfirmed)
            return BadRequest("This email is already confirmed");

        token = Uri.UnescapeDataString(token);

        IdentityResult Result = await _userManager.ConfirmEmailAsync(AuthUser, token);
        if (!Result.Succeeded)
            return BadRequest("Failed to verify account");

        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);

        return Ok("Account verified, you can now close this page :)");
    }

    [HttpPost("/auth/account/changePassword")]
    public async Task<IActionResult> ChangePassword([FromHeader] string requestId, [FromForm] string email, [FromForm] string password, [FromForm] string emailToken)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(requestId))
            return BadRequest("Request is invalid");

        if (string.IsNullOrEmpty(email))
            return BadRequest("Email is invalid");

        UserSessionJson? Data = Cache.Get<UserSessionJson>("changepass-" + requestId);

        if (Data == null || Data.Email != email)
            return BadRequest("Request is invalid");

        if (string.IsNullOrEmpty(password))
            return BadRequest("Password is invalid");

        if (string.IsNullOrEmpty(emailToken))
            return BadRequest("Email token is invalid");

        AuthUser? User = await _signInManager.UserManager.FindByEmailAsync(email);
        if (User == null)
            return BadRequest("Failed to change password");

        User.Account.PasswordChangedAt = DateTime.UtcNow;
        User.Account.PasswordStrength = Utils.GetPasswordStrength(password);

        IdentityResult Result = await _userManager.ResetPasswordAsync(User, emailToken, password);
        if (!Result.Succeeded)
            return BadRequest("Failed to change password");

        await Email.SendPasswordChanged(User, "https://" + Request.Host.Value);

        Microsoft.AspNetCore.Identity.SignInResult Signin = await _signInManager.PasswordSignInAsync(User, password, false, false);
        if (!Signin.Succeeded)
            return BadRequest("Failed to login");

        User.Account.PasswordChangedAt = DateTime.UtcNow;
        string? Ip = Utils.GetUserIpAddress(Request.HttpContext);
        if (string.IsNullOrEmpty(Ip))
            return BadRequest("IP address failed.");

        string? SessionId = Request.Cookies["DevSpace.SessionId"];
        if (string.IsNullOrEmpty(SessionId))
            return BadRequest("Invalid session.");

        if (User.Account.Sessions.TryGetValue(SessionId, out UserSession? session))
        {
            session.LastLoginAt = DateTime.UtcNow;
            session.AuthorizedIps.Add(Utils.GetStringSha256Hash(Ip));
        }
        else
        {
            UserSession Session = UserSession.Create(Data);
            Session.AuthorizedIps.Add(Utils.GetStringSha256Hash(Ip));
            User.Account.Sessions.TryAdd(SessionId, Session);
        }

        await _userManager.UpdateAsync(User);


        return Ok();
    }

}
