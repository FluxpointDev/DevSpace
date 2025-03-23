using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class ClientController : APIController
{
    [HttpGet("/api/teams/{teamId?}/clients")]
    [SwaggerOperation("Get a list of API clients.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetClients([FromRoute] string teamId = "")
    {
        if (string.IsNullOrEmpty(teamId) || !ObjectId.TryParse(teamId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(obj, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        if (!Client.HasAPIPermission(Team, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(Team, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        return Ok(_DB.API.Cache.Values.Where(x => x.TeamId.ToString() == teamId && (Client.HasAPIPermission(Team, APIPermission.ViewAllAPIs) || Client.OwnerId == x.OwnerId)).Select(x => new ClientJson(x)));
    }

    [HttpGet("/api/clients/{clientId}")]
    [SwaggerOperation("Get a list of API clients.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetClient([FromRoute] string clientId = "")
    {
        if (!_DB.Teams.Cache.TryGetValue(Client.TeamId.Value, out Data.Teams.TeamData? Team))
            return BadRequest("Could not find team.");

        if (!Client.HasAPIPermission(Team, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(Team, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out var client))
            return BadRequest("Could not find client.");



        if (!Client.IsInstanceAdmin && Client.TeamId != client.TeamId)
            return BadRequest("Could not find client.");

        if (client.Id != obj && !(Client.HasAPIPermission(Team, APIPermission.ViewAllAPIs) || Client.OwnerId == client.OwnerId))
            return BadRequest("Could not find client.");

        return Ok(new ClientJson(client));
    }

    [HttpGet("/api/clients/@me")]
    [SwaggerOperation("Get the current API clients.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson>))]
    public async Task<IActionResult> GetCurrentClient()
    {
        return Ok(new ClientJson(Client));
    }

    [HttpGet("/api/clients/@me/team")]
    [SwaggerOperation("Get the current API client team.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson>))]
    public async Task<IActionResult> GetCurrentClientTeam()
    {
        if (!Client.TeamId.HasValue)
            return BadRequest("This api client has no team.");

        if (!_DB.Teams.Cache.TryGetValue(Client.TeamId.Value, out var team))
            return NotFound("Could not find team for the client.");

        return Ok(new TeamJson(team, Client.HasTeamPermission(team, TeamPermission.ViewPermissions)));
    }

    [HttpGet("/api/clients/@me/owner")]
    [SwaggerOperation("Get the current API client owner.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson>))]
    public async Task<IActionResult> GetCurrentClientOwner()
    {
        if (!Client.TeamId.HasValue)
            return BadRequest("This api client has no team.");

        AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, Client.OwnerId)).FirstOrDefaultAsync();
        if (user == null)
            return BadRequest("Could not find user.");

        return Ok(new UserJson(user));
    }
}
