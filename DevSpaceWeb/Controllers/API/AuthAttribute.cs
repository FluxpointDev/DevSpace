using Microsoft.AspNetCore.Mvc.Filters;
using DevSpaceWeb.Data.Permissions;

namespace DevSpaceWeb.Controllers.API;

public class IsAuthenticatedAttribute : ActionFilterAttribute
{
    public IsAuthenticatedAttribute()
    {

    }

    public IsAuthenticatedAttribute(TeamPermission permission)
    {
        TeamPermission = permission;
    }

    public TeamPermission? TeamPermission { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        APIController controller = filterContext.Controller as APIController;
        if (!filterContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
        {
            filterContext.Result = controller.CustomStatus(401, "Authorization header is missing.");
            return;
        }

        string AuthKey = filterContext.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(AuthKey))
        {
            filterContext.Result = controller.CustomStatus(401, "Authorization header is empty.");
            return;
        }
        filterContext.Result = controller.CustomStatus(401, "You are not allowed to use the API.");
        return;
        //if (!_DB.Keys.TryGetValue(AuthKey, out ApiUser User))
        //{
        //    filterContext.Result = controller.CustomStatus(401, "Your token is invalid, for support go to " + Config.Discord + " ( G-2 )");
        //    return;
        //}

        //if (User.Disabled)
        //{
        //    filterContext.Result = controller.CustomStatus(403, "Your token is disabled, for support go to " + Config.Discord + " ( G-3 )");
        //    return;
        //}


        //controller.User = User;
    }
}
