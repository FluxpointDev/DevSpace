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
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class RolesController : APIController
{
    [HttpGet("/api/teams/{teamId?}/roles")]
    [SwaggerOperation("Get a list of role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<RoleJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetRoles([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewRoles, out var perm))
            return PermissionFailed(perm);

        return Ok(Team.CachedRoles.Values.Select(x => new RoleJson(x)));
    }

    [HttpGet("/api/roles/{roleId?}")]
    [SwaggerOperation("Get a role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<RoleJson>))]
    public async Task<IActionResult> GetRole([FromRoute] string roleId = "")
    {
        if (string.IsNullOrEmpty(roleId) || !ObjectId.TryParse(roleId, out ObjectId obj) || !_DB.Roles.Cache.TryGetValue(obj, out Data.Teams.TeamRoleData? Role) || !(Client.IsInstanceAdmin || Role.TeamId == Client.TeamId.GetValueOrDefault()))
            return BadRequest("Could not find role.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewRoles, out var perm))
            return PermissionFailed(perm);

        return Ok(new RoleJson(Role));
    }



    //[HttpPost("/api/roles/{roleId?}")]
    //[SwaggerOperation("Edit a role.", "")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<RoleJson>))]
    //public async Task<IActionResult> EditRole([FromRoute] string roleId = "")
    //{
    //    if (string.IsNullOrEmpty(roleId) || !ObjectId.TryParse(roleId, out ObjectId obj) || !_DB.Roles.Cache.TryGetValue(obj, out Data.Teams.TeamRoleData? Role) || !(Client.IsInstanceAdmin || Role.TeamId == Client.TeamId.GetValueOrDefault()))
    //        return BadRequest("Could not find role.");

    //    if (!Client.HasTeamPermission(TeamPermission.ViewRoles))
    //        return Forbidden("Client does not have View Roles permission.");

    //    if (!Client.HasTeamPermission(TeamPermission.ManageRoles))
    //        return Forbidden("Client does not have Manage Roles permission.");

    //    return Ok();
    //}
}
