using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;

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

        string Authorization = filterContext.HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(Authorization))
        {
            filterContext.Result = controller.CustomStatus(401, "Authorization header is empty.");
            return;
        }

        string[] Parts = Authorization.Split('.', StringSplitOptions.TrimEntries);
        if (Parts.Length != 2)
        {
            filterContext.Result = controller.CustomStatus(401, "Invalid client.");
            return;
        }
        string ClientId = Parts[0];
        string Auth = Parts[1];

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

        var Validate = Utils.Hasher.VerifyHashedPassword(null, controller.Client.TokenHash, Auth);

        if (Validate == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            filterContext.Result = controller.CustomStatus(401, "Client authentication has failed.");
            return;
        }

        if (controller.Client.IsDisabled)
        {
            filterContext.Result = controller.CustomStatus(403, "Client has been disabled.");
            return;
        }


        //controller.User = User;
    }
}
