using DevSpaceWeb.API.Teams;
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
}
