using Microsoft.AspNetCore.Mvc.Filters;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data;
using MongoDB.Bson;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Database;

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
        if (!_Data.Config.Instance.Features.APIEnabled)
        {
            filterContext.Result = controller.CustomStatus(401, "API access has been disabled on this instance.");
            return;
        }
        if (!filterContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
        {
            filterContext.Result = controller.CustomStatus(401, "Authorization header is missing.");
            return;
        }

        if (!filterContext.HttpContext.Request.Headers.ContainsKey("Client-Id"))
        {
            filterContext.Result = controller.CustomStatus(401, "Client-Id header is missing.");
            return;
        }

        string AuthKey = filterContext.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(AuthKey))
        {
            filterContext.Result = controller.CustomStatus(401, "Authorization header is empty.");
            return;
        }

        string ClientId = filterContext.HttpContext.Request.Headers["Client-Id"];
        if (string.IsNullOrEmpty(ClientId))
        {
            filterContext.Result = controller.CustomStatus(401, "Client-Id header is empty.");
            return;
        }

        if (ObjectId.TryParse(ClientId, out ObjectId id))
            _DB.API.Cache.TryGetValue(id, out controller.Client);

        if (controller.Client == null)
        {
            filterContext.Result = controller.CustomStatus(401, "Invalid client.");
            return;
        }

        if (string.IsNullOrEmpty(controller.Client.TokenHash))
        {
            filterContext.Result = controller.CustomStatus(401, "Client authentication has failed.");
            return;
        }

        var Validate = Utils.Hasher.VerifyHashedPassword(null, controller.Client.TokenHash, AuthKey);

        if (Validate == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            filterContext.Result = controller.CustomStatus(401, "Client authentication has failed.");
            return;
        }

        if (controller.Client.IsDisabled)
        {
            filterContext.Result = controller.CustomStatus(401, "Client has been disabled.");
            return;
        }

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
