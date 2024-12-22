using DevSpaceWeb.API.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
public class TeamController : APIController
{
    [HttpGet("/api/teams/{teamId?}")]
    public async Task<IActionResult> GetTeam([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out var obj) || !_DB.Teams.Cache.TryGetValue(obj, out var Team))
            return BadRequest("Could not find team.");

        return Ok(new TeamJson(Team));
    }
}
