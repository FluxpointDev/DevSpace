using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.Teams;
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
public class TeamsController : APIController
{
    [HttpGet("/api/teams/{teamId?}")]
    [SwaggerOperation("Get a team.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(TeamJson))]
    public async Task<IActionResult> GetTeam([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        return Ok(new TeamJson(Team));
    }

    

    
}
