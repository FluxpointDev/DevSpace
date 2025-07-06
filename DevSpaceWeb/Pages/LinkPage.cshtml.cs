using DevSpaceWeb.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevSpaceWeb.Pages;

public class LinkPageModel : PageModel
{
    public LinkPageModel(UserManager<AuthUser> userManager)
    {
        UserManager = userManager;
    }

    public UserManager<AuthUser> UserManager;

    public bool RequireAuth;
    public bool RequireMember;
    public bool RequireRoles;

    public bool IsAuthenticated;
    public bool IsMember;
    public bool HasRoles;

    public async Task OnGet()
    {
        string[] Split = Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (Split.Length != 3)
        {
            await Response.WriteAsync("Invalid link request.");
            return;
        }
        string TeamId = Split[1];
        string ShortCode = Split[2];
        if (string.IsNullOrEmpty(TeamId))
        {
            await Response.WriteAsync("Team id is missing.");
            return;
        }
        if (string.IsNullOrEmpty(ShortCode))
        {
            await Response.WriteAsync("Code is missing.");
            return;
        }


        Console.WriteLine("Code: " + ShortCode);

        RequireAuth = true;
        AuthUser? User = null;

        if (Request.HttpContext.User.Identity != null && Request.HttpContext.User.Identity.IsAuthenticated)
            User = await UserManager.GetUserAsync(Request.HttpContext.User);

        if (User != null && User.Disabled == null)
            IsAuthenticated = true;




    }
}
