using DevSpaceWeb.Data.Links;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
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
        if (LinkData.Static.RequireAuthentication)
        {
            RequireAuth = true;
            AuthUser? User = null;

            if (Request.HttpContext.User.Identity != null && Request.HttpContext.User.Identity.IsAuthenticated)
                User = await UserManager.GetUserAsync(Request.HttpContext.User);

            if (User != null && User.Disabled == null)
                IsAuthenticated = true;

            if (LinkData.Static.RequireMember)
            {
                RequireMember = true;

                TeamMemberData? Member = _DB.Teams.Cache.Values.First().GetMember(User);
                if (Member != null)
                    IsMember = true;

                if (LinkData.Static.RequireRoles)
                {
                    RequireRoles = true;
                    if (Member != null && Member.Roles.Count != 0)
                        HasRoles = true;

                }
            }
        }

    }
}
