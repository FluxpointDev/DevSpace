using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using DevSpaceWeb.Services;
using DevSpaceWeb.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace DevSpaceWeb.Controllers;

public class AuthAccountController : AuthControllerContext
{
    public AuthAccountController(
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service, IDistributedCache cache, EmailService email) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
        Email = email;
    }

    private readonly IDistributedCache Cache;
    private readonly EmailService Email;

    // Download Recovery Code
    [HttpGet("/auth/account/downloadRecoveryCode")]
    public async Task<IActionResult> RecoveryCode([FromQuery] string token = "")
    {
        if (!User.Identity.IsAuthenticated)
            return Redirect("/login");

        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(token))
            return BadRequest("Invalid token");

        string? CodeString = await Cache.GetStringAsync(token);
        if (string.IsNullOrEmpty(CodeString))
            return BadRequest("Invalid token or expired");

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);

        if (AuthUser == null)
            return BadRequest("Invalid user account");

        if (string.IsNullOrEmpty(AuthUser.Auth.RecoveryCode))
            return BadRequest("You do not have any recovery codes");


        string data = CodeString;
        var content = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(data));
        var contentType = "APPLICATION/octet-stream";
        var fileName = $"{AuthUser.Email.Replace(".", "_")} Codes.txt";
        return File(content, contentType, fileName);
    }

    [HttpGet("/auth/account/confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token = "")
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(token))
            return BadRequest("Invalid token");

        if (!User.Identity.IsAuthenticated)
            return Redirect("/login?token=" + token);

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);
        if (AuthUser == null)
            return BadRequest("Invalid user account");

        if (AuthUser.EmailConfirmed)
            return BadRequest("This email is already confirmed");

        token = Uri.UnescapeDataString(token);

        var Result = await _userManager.ConfirmEmailAsync(AuthUser, token);
        if (!Result.Succeeded)
            return BadRequest("Failed to verify account");

        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);

        return Ok("Account verified, you can now close this page :)");
    }

    [HttpGet("/auth/account/changeEmail")]
    public async Task<IActionResult> ChangeEmail([FromQuery] string email = "", [FromQuery] string token = "")
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(email))
            return BadRequest("Invalid email");

        if (string.IsNullOrEmpty(token))
            return BadRequest("Invalid token");

        if (!User.Identity.IsAuthenticated)
            return Redirect("/login");

        AuthUser? AuthUser = await _userManager.GetUserAsync(User);

        if (AuthUser == null)
            return BadRequest("Invalid user account");

        token = Uri.UnescapeDataString(token);

        var Result = await _userManager.ChangeEmailAsync(AuthUser, email, token);
        if (!Result.Succeeded)
            return BadRequest("Failed to verify account");

        AuthUser.Auth.EmailChangedAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(AuthUser);

        _DB.TriggerSessionEvent(AuthUser.Id, SessionEventType.AccountUpdate);

        return Ok("Account email changed, you can now close this page :)");
    }

    [HttpPost("/auth/account/changePassword")]
    public async Task<IActionResult> ChangePassword([FromHeader] string requestId, [FromForm] string email, [FromForm] string password, [FromForm] string emailToken)
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        if (string.IsNullOrEmpty(requestId))
            return BadRequest("Request id is invalid");

        if (string.IsNullOrEmpty(email))
            return BadRequest("Email is invalid");

        string Data = Cache.GetString("changepass-" + requestId);

        if (string.IsNullOrEmpty(Data) || Data != email)
            return BadRequest("Request id is invalid");

        if (string.IsNullOrEmpty(password))
            return BadRequest("Password is invalid");

        if (string.IsNullOrEmpty(emailToken))
            return BadRequest("Email token is invalid");

        var User = await _signInManager.UserManager.FindByEmailAsync(email);
        if (User == null)
            return BadRequest("Failed to change password");

        var Result = await _userManager.ResetPasswordAsync(User, emailToken, password);
        if (!Result.Succeeded)
            return BadRequest("Failed to change password");

        await Email.SendPasswordChanged(User, "https://" + Request.Host.Value);

        var Signin = await _signInManager.PasswordSignInAsync(User, password, false, false);
        if (!Signin.Succeeded)
            return BadRequest("Failed to login");

        User.Auth.PasswordChangedAt = DateTimeOffset.UtcNow;
        string Ip = Utils.GetUserIpAddress(Request.HttpContext);
        if (!string.IsNullOrEmpty(Ip))
            User.Auth.Sessions.Add(Utils.GetStringSha256Hash(Ip), new AuthUserSession { });

        await _userManager.UpdateAsync(User);


        return Ok();
    }

}
