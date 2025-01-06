using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class RolesController : APIController
{
    [HttpGet("/api/roles/{roleId?}")]
    [SwaggerOperation("Get a role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleJson))]
    public async Task<IActionResult> GetRole([FromRoute] string roleId = "")
    {
        if (string.IsNullOrEmpty(roleId) || !ObjectId.TryParse(roleId, out ObjectId obj) || !_DB.Roles.Cache.TryGetValue(obj, out Data.Teams.TeamRoleData? Role) && Role.TeamId != Client.TeamId)
            return BadRequest("Could not find role.");

        return Ok(new RoleJson(Role));
    }

    [HttpGet("/api/teams/{teamId?}/roles")]
    [SwaggerOperation("Get a list of role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleJson[]))]
    public async Task GetRoles([FromRoute] string teamId = "")
    {

    }

    [HttpPost("/api/teams/{teamId?}/roles")]
    [SwaggerOperation("Edit a role.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleJson))]
    public async Task CreateRole([FromRoute] string teamId = "")
    {

    }
}
