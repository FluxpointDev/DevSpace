using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Requires permission View Roles")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class RolesController : APIController
{
    [HttpGet("/api/roles")]
    [SwaggerOperation("Get a list of role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<RoleJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetRoles()
    {
        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewRoles, out var perm))
            return PermissionFailed(perm);

        bool HasViewPerms = Client.HasTeamPermission(CurrentTeam, TeamPermission.ViewPermissions);

        return Ok(CurrentTeam.CachedRoles.Values.Select(x => new RoleJson(x, HasViewPerms)).OrderBy(x => x.position));
    }

    [HttpGet("/api/roles/{roleId?}")]
    [SwaggerOperation("Get a role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<RoleJson>))]
    public async Task<IActionResult> GetRole([FromRoute] string roleId = "")
    {
        if (string.IsNullOrEmpty(roleId) || !ObjectId.TryParse(roleId, out ObjectId obj) || !_DB.Roles.Cache.TryGetValue(obj, out Data.Teams.TeamRoleData? Role) || !(Role.TeamId == Client.TeamId))
            return NotFound("Could not find role.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewRoles, out var perm))
            return PermissionFailed(perm);

        bool HasViewPerms = Client.HasTeamPermission(CurrentTeam, TeamPermission.ViewPermissions);

        return Ok(new RoleJson(Role, HasViewPerms));
    }
}
