using DevSpaceWeb.API;
using DevSpaceWeb.API.Apps;
using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Manage Team apps and information.")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class AppsController : APIController
{
    [HttpGet("/api/apps/{appId?}")]
    [SwaggerOperation("Get a app.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<AppJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetApp([FromRoute] string appId = "")
    {
        if (string.IsNullOrEmpty(appId) || !ObjectId.TryParse(appId, out ObjectId obj) || !_DB.Apps.Cache.TryGetValue(obj, out AppData? app) || !(app.TeamId == Client.TeamId))
            return NotFound("Could not find app.");

        if (Client.CheckFailedAppPermissions(app, AppPermission.ViewApp, out AppPermission? perm))
            return PermissionFailed(perm!);

        return Ok(new AppJson(app));
    }
}
