using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using DevSpaceShared.Responses;
using DevSpaceWeb.API;
using DevSpaceWeb.API.Servers;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Requires permission View Servers")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class DockerController : APIController
{
    [HttpGet("/api/servers/{serverId?}/system")]
    [SwaggerOperation("Get server system info.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerSystemJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServerSystem([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        var Response = await server.RecieveJsonAsync<DevSpaceShared.Responses.SystemInfoResponse>(new DockerEvent(DockerEventType.SystemInfo));
        if (!Response.IsSuccess)
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

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer | ServerPermission.ViewHostInfo, out var perm))
            return PermissionFailed(perm);

        var Response = await server.RecieveJsonAsync<SystemInfoFullResponse>(new DockerEvent(DockerEventType.HostInfo));
        if (!Response.IsSuccess)
            return Conflict("Failed to get server data, " + Response.Message);

        return Ok(new ServerHostJson(server, Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/stacks")]
    [SwaggerOperation("Get server stacks list.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetStacks([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out var dockerPerm))
            return PermissionFailed(dockerPerm);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewStacks, out var dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm);

        var Response = await server.RecieveJsonAsync<List<DockerStackInfo>>(new DockerEvent(DockerEventType.ListStacks));

        if (!Response.IsSuccess)
            return Conflict("Failed to get stacks data, " + Response.Message);

        return Ok(Response.Data.Select(x => new StackJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/containers")]
    [SwaggerOperation("Get server containers list.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetContainers([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out var dockerPerm))
            return PermissionFailed(dockerPerm);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out var dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm);

        var Response = await server.RecieveJsonAsync<List<DockerContainerInfo>>(new DockerEvent(DockerEventType.ListContainers));

        if (!Response.IsSuccess)
            return Conflict("Failed to get containers data, " + Response.Message);

        return Ok(Response.Data.Select(x => new ContainerJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/stacks/{stackId?}")]
    [SwaggerOperation("Get server stack info.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetStack([FromRoute] string serverId = "", [FromRoute] string stackId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(stackId))
            return BadRequest("Stack id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out var dockerPerm))
            return PermissionFailed(dockerPerm);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out var dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm);

        var Response = await server.RecieveJsonAsync<DockerStackInfo>(new DockerEvent(DockerEventType.ControlStack, stackId, stackType: ControlStackType.View));
        if (!Response.IsSuccess)
            return Conflict("Failed to get stack data, " + Response.Message);

        if (Response.Data == null || string.IsNullOrEmpty(Response.Data.Id))
            return NotFound("Stack does not exist");

        return Ok(new StackJson(Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}")]
    [SwaggerOperation("Get server container info.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetContainers([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out var dockerPerm))
            return PermissionFailed(dockerPerm);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out var dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm);

        var Response = await server.RecieveJsonAsync<ContainerInspectResponse>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Inspect));
        if (!Response.IsSuccess)
            return Conflict("Failed to get container data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Container does not exist");

        return Ok(new ContainerJson(Response.Data));
    }

    [HttpPatch("/api/servers/{serverId?}/containers/{containerId?}/control")]
    [SwaggerOperation("Control server container state.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    [ListParameterSwaggerAttribute("type", ["start", "stop", "restart", "pause", "unpause", "kill"])]
    public async Task<IActionResult> ControlContainer([FromRoute] string serverId = "", [FromRoute] string containerId = "", [FromQuery][Required] string? type = null)
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (string.IsNullOrEmpty(type))
            return BadRequest("Control type parameter is missing from query ?type=stop");

        if (!Enum.TryParse(type, true, out ControlContainerType control))
            return BadRequest("Invalid control type parameter.");

        switch (control)
        {
            case ControlContainerType.Changes:
            case ControlContainerType.ForceRemove:
            case ControlContainerType.Inspect:
            case ControlContainerType.Logs:
            case ControlContainerType.Processes:
            case ControlContainerType.Remove:
            case ControlContainerType.Update:
            case ControlContainerType.View:
                return BadRequest("Invalid control type parameter.");
        }

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out var perm))
            return PermissionFailed(perm);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out var dockerPerm))
            return PermissionFailed(dockerPerm);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ControlContainers, out var dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm);

        var Response = await server.RecieveJsonAsync<ContainerInspectResponse>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: control));
        if (!Response.IsSuccess)
            return Conflict("Failed to control container, " + Response.Message);

        return Ok();
    }
}
