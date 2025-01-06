using DevSpaceWeb.API.Teams;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
public class TeamController : APIController
{
    [HttpGet("/api/teams/{teamId?}")]
    public async Task<IActionResult> GetTeam([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        return Ok(new TeamJson(Team));
    }

    [HttpGet("/api/roles/{roleId?}")]
    public async Task<IActionResult> GetRole([FromRoute] string roleId = "")
    {
        if (string.IsNullOrEmpty(roleId) || !ObjectId.TryParse(roleId, out ObjectId obj) || !_DB.Roles.Cache.TryGetValue(obj, out Data.Teams.TeamRoleData? Role) && Role.TeamId != Client.TeamId)
            return BadRequest("Could not find role.");

        return Ok(new RoleJson(Role));
    }

    [HttpGet("/api/members/{memberId?}")]
    public async Task<IActionResult> GetUser([FromRoute] string memberId = "")
    {
        if (string.IsNullOrEmpty(memberId) || !ObjectId.TryParse(memberId, out ObjectId obj) || !_DB.Members.Cache.TryGetValue(obj, out TeamMemberData member) || member.TeamId != Client.TeamId)
            return BadRequest("Could not find member.");

        return Ok(new MemberJson(member));
    }
}
