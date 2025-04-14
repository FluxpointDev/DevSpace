using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Radzen;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Manage Team API clients.")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class ClientController : APIController
{
    [HttpGet("/api/clients")]
    [SwaggerOperation("Get a list of API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetClients()
    {
        if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        return Ok(_DB.API.Cache.Values.Where(x => x.TeamId == Client.TeamId && (Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.OwnerId == x.OwnerId)).Select(x => new ClientJson(x)));
    }

    [HttpGet("/api/clients/{clientId}")]
    [SwaggerOperation("Get an API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetClient([FromRoute] string clientId = "")
    {
        if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out APIClient? client))
            return NotFound("Could not find client.");

        if (Client.TeamId != client.TeamId)
            return NotFound("Could not find client.");

        if (client.Id != obj && !(Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs)))
            return NotFound("Could not find client.");

        return Ok(new ClientJson(client));
    }

    [HttpGet("/api/clients/@me")]
    [SwaggerOperation("Get the current API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ClientJson>))]
    public async Task<IActionResult> GetCurrentClient()
    {
        return Ok(new ClientJson(Client));
    }

    [HttpGet("/api/clients/@me/team")]
    [SwaggerOperation("Get the current API client team.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson>))]
    public async Task<IActionResult> GetCurrentClientTeam()
    {
        return Ok(new TeamJson(CurrentTeam, Client.HasTeamPermission(CurrentTeam, TeamPermission.ViewPermissions)));
    }

    [HttpGet("/api/clients/@me/owner")]
    [SwaggerOperation("Get the current API client owner.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<UserJson>))]
    public async Task<IActionResult> GetCurrentClientOwner()
    {
        AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, Client.OwnerId)).FirstOrDefaultAsync();
        if (user == null)
            return Conflict("Could not find owner user.");

        return Ok(new UserJson(user));
    }

    [HttpPatch("/api/clients/{clientId}/enable")]
    [SwaggerOperation("Enable the API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> EnableClient([FromRoute] string clientId = "")
    {
        if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out APIClient? client))
            return NotFound("Could not find API client.");

        if (Client.TeamId != client.TeamId)
            return NotFound("Could not find API client.");

        if (client.Id != obj && !(Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.OwnerId == client.OwnerId))
            return NotFound("Could not find client.");

        if (client.Id != obj && !Client.CanManage(CurrentOwner, checkAllowedPermissions: true))
            return BadRequest("You can't manage this API client.");

        if (!Client.IsDisabled)
            return Ok("API Client is already enabled.");

        UpdateResult Result = await Client.UpdateAsync(new UpdateDefinitionBuilder<APIClient>().Set(x => x.IsDisabled, false));
        if (Result.IsAcknowledged)
            return Ok("API Client has been enabled.");
        else
            return Conflict("Failed to enable API Client.");
    }

    //[HttpPatch("/api/clients/{clientId}/owner")]
    //[SwaggerOperation("Update the owner of the API client.")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<UserJson>))]
    //[SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    //public async Task<IActionResult> GetClientOwner([FromRoute] string clientId = "")
    //{
    //    if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
    //        return PermissionFailed(APIPermission.ViewOwnAPIs);

    //    if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out APIClient? client))
    //        return NotFound("Could not find API client.");

    //    if (Client.TeamId != client.TeamId)
    //        return NotFound("Could not find API client.");

    //    if (client.Id != obj && !(Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.OwnerId == client.OwnerId))
    //        return PermissionFailed(APIPermission.ViewOwnAPIs);

    //    if (client.Id != obj && !Client.HasTeamPermission(CurrentTeam, TeamPermission.ViewMembers))
    //        return PermissionFailed(TeamPermission.ViewMembers);

    //    AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, client.OwnerId)).FirstOrDefaultAsync();
    //    if (user == null)
    //        return Conflict("Could not find owner user.");

    //    return Ok(new UserJson(user));
    //}

    [HttpPatch("/api/clients/{clientId}/disable")]
    [SwaggerOperation("Disable the API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> DisableClient([FromRoute] string clientId = "")
    {
        if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out APIClient? client))
            return NotFound("Could not find API client.");

        if (Client.TeamId != client.TeamId)
            return NotFound("Could not find API client.");

        if (client.Id != obj && !(Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.OwnerId == client.OwnerId))
            return NotFound("Could not find client.");

        if (client.Id != obj && !Client.CanManage(CurrentOwner, checkAllowedPermissions: true))
            return BadRequest("You can't manage this API client.");

        if (Client.IsDisabled)
            return Ok("API Client is already disabled.");

        UpdateResult Result = await Client.UpdateAsync(new UpdateDefinitionBuilder<APIClient>().Set(x => x.IsDisabled, true));
        if (Result.IsAcknowledged)
            return Ok("API Client has been disabled.");
        else
            return Conflict("Failed to disable API Client.");
    }

    [HttpDelete("/api/clients/{clientId}/delete")]
    [SwaggerOperation("Delete the API clients.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> DeleteClient([FromRoute] string clientId = "")
    {
        if (!Client.HasAPIPermission(CurrentTeam, APIPermission.ViewOwnAPIs) && !Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs))
            return PermissionFailed(APIPermission.ViewOwnAPIs);

        if (string.IsNullOrEmpty(clientId) || !ObjectId.TryParse(clientId, out ObjectId obj) || !_DB.API.Cache.TryGetValue(obj, out APIClient? client))
            return NotFound("Could not find API client.");

        if (Client.TeamId != client.TeamId)
            return NotFound("Could not find API client.");

        if (client.Id != obj && !(Client.HasAPIPermission(CurrentTeam, APIPermission.ViewAllAPIs) || Client.OwnerId == client.OwnerId))
            return NotFound("Could not find client.");

        if (client.Id != obj && !Client.CanManage(CurrentOwner, checkAllowedPermissions: true))
            return BadRequest("You can't manage this API client.");

        DeleteResult Result = await Client.DeleteAsync();
        if (Result.IsAcknowledged)
            return Ok("API Client has been deleted.");
        else
            return Conflict("Failed to delete API Client.");
    }
}
