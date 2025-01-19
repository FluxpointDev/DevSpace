using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
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
public class MembersController : APIController
{
    [HttpGet("/api/teams/{teamId?}/members")]
    [SwaggerOperation("Get a list of members.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<MemberJson[]>))]
    public async Task<IActionResult> GetMembers([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewMembers, out var perm))
            return PermissionFailed(perm);

        return Ok(Team.CachedMembers.Values.Select(x => new MemberJson(x)));
    }

    [HttpGet("/api/teams/{teamId}/members/{userId?}")]
    [SwaggerOperation("Get a member.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<MemberJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetMember([FromRoute] string teamId = "", [FromRoute] string userId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team) || !(Client.IsInstanceAdmin || Team.Id == Client.TeamId.GetValueOrDefault()))
            return BadRequest("Could not find team.");

        if (string.IsNullOrEmpty(userId) || !ObjectId.TryParse(userId, out ObjectId obj2) || !Team.Members.TryGetValue(obj2, out ObjectId memberObj) || !Team.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
            return NotFound("Could not find member.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewMembers, out var perm))
            return PermissionFailed(perm);

        return Ok(new MemberJson(member));
    }


}
