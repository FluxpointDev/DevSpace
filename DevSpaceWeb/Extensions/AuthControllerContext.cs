using DevSpaceWeb.Data;
using DevSpaceWeb.Fido2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb;

public class AuthControllerContext : Controller
{
    public readonly UserManager<AuthUser> _userManager;
    public readonly SignInManager<AuthUser> _signInManager;
    public readonly Fido2Service _fido2Service;
    public AuthControllerContext(UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        Fido2Service fido2Service)
    {
        _userManager = userManager;
        _fido2Service = fido2Service;
        _signInManager = signInManager;
    }

    public Task<AuthUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(User);
}
