using DevSpaceShared.Events.Docker;
using DevSpaceShared.Responses;
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
[SwaggerTag("Manage Team servers and information.")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class ServersController : APIController
{
    [HttpGet("/api/servers")]
    [SwaggerOperation("Get a list of servers.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerJson[]>))]
    public async Task<IActionResult> GetServers([FromQuery] bool showIp = false)
    {
        return Ok(_DB.Servers.Cache.Values.Where(x => (x.TeamId == Client.TeamId) && Client.HasServerPermission(CurrentTeam, x, ServerPermission.ViewServer)).Select(x => new ServerJson(x, showIp && Client.HasServerPermission(CurrentTeam, x, ServerPermission.ViewHostInfo))));
    }

    [HttpGet("/api/servers/{serverId?}")]
    [SwaggerOperation("Get a server.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServer([FromRoute] string serverId = "", [FromQuery] bool showIp = false)
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (showIp)
        {
            if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewHostInfo, out perm))
                return PermissionFailed(perm!);
        }

        return Ok(new ServerJson(server, showIp));
    }

    [HttpGet("/api/servers/{serverId?}/system")]
    [SwaggerOperation("Get server system info.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerSystemJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServerSystem([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        SocketResponse<DevSpaceShared.Responses.SystemInfoResponse?> Response = await server.RecieveJsonAsync<DevSpaceShared.Responses.SystemInfoResponse>(new DockerEvent(DockerEventType.SystemInfo));
        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get server data, " + Response.Message);

        return Ok(new ServerSystemJson(server, Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/host")]
    [SwaggerOperation("Get server host details.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerHostJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServerHost([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer | ServerPermission.ViewHostInfo, out ServerPermission? perm))
            return PermissionFailed(perm!);

        SocketResponse<SystemInfoFullResponse?> Response = await server.RecieveJsonAsync<SystemInfoFullResponse>(new DockerEvent(DockerEventType.HostInfo));
        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get server data, " + Response.Message);

        return Ok(new ServerHostJson(server, Response.Data));
    }

}
