using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
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
        Fido2Service fido2Service, IDistributedCache cache) : base(userManager, signInManager, fido2Service)
    {
        Cache = cache;
    }

    public IDistributedCache Cache { get; private set; }

    // Download Recovery Code
    [HttpGet("/api/account/recoveryCode")]
    public async Task<IActionResult> RecoveryCode([FromQuery] string token = "")
    {
        if (!User.Identity.IsAuthenticated)
            return Redirect("/login");

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

    [HttpGet("/api/account/confirm")]
    public async Task<IActionResult> ConfirmAccount([FromQuery] string token = "")
    {
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

        AuthUser.Auth.IsTwoFactorEnabled = true;
        await _userManager.UpdateAsync(AuthUser);

        return Ok("Account verified, you can now close this page :)");
    }

    [HttpGet("/api/account/changeEmail")]
    public async Task<IActionResult> ChangeEmail([FromQuery] string email = "", [FromQuery] string token = "")
    {
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

        return Ok("Account email changed, you can now close this page :)");
    }

}
