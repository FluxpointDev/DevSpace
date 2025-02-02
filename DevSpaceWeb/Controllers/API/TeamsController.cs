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
public class TeamsController : APIController
{
    [HttpGet("/api/teams")]
    [SwaggerOperation("Get all teams for this instance.", "This requires instance admin.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    [RequireInstanceAdmin]
    public async Task<IActionResult> GetTeams([FromRoute] string teamId = "")
    {
        if (!Client.IsInstanceAdmin)
            return Forbidden("Client does not have Instance Admin privilages.");

        return Ok(_DB.Teams.Cache.Values.Select(x => new TeamJson(x, true)));
    }

    [HttpGet("/api/teams/{teamId?}")]
    [SwaggerOperation("Get a team.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetTeam([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team) || !(Client.IsInstanceAdmin || Team.Id == Client.TeamId.GetValueOrDefault()))
            return BadRequest("Could not find team.");

        return Ok(new TeamJson(Team, Client.HasTeamPermission(TeamPermission.ViewPermissions)));
    }




}
