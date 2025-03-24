using DevSpaceWeb.API;
using DevSpaceWeb.API.Servers;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Requires permission View Servers")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class ServersController : APIController
{
    [HttpGet("/api/servers")]
    [SwaggerOperation("Get a list of servers.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerJson[]>))]
    public async Task<IActionResult> GetServers()
    {
        return Ok(_DB.Servers.Cache.Values.Where(x => (x.TeamId == Client.TeamId) && Client.HasServerPermission(CurrentTeam, x, ServerPermission.ViewServer)).Select(x => new ServerJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}")]
    [SwaggerOperation("Get a server.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServer([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        return Ok(new ServerJson(server));
    }


}
