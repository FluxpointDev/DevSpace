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
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Requires View Server permission.")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class DockerController : APIController
{
    [HttpGet("/api/servers/{serverId?}/stacks")]
    [SwaggerOperation("Get server stacks list.", "Requires View Server and View Stack permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetStacks([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewStacks, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<List<DockerStackInfo>?> Response = await server.RecieveJsonAsync<List<DockerStackInfo>>(new DockerEvent(DockerEventType.ListStacks));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get stacks data, " + Response.Message);

        return Ok(Response.Data.Select(x => new StackJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/containers")]
    [SwaggerOperation("Get server containers list.", "Requires View Server and View Container permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetContainers([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<List<DockerContainerInfo>?> Response = await server.RecieveJsonAsync<List<DockerContainerInfo>>(new DockerEvent(DockerEventType.ListContainers));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get containers data, " + Response.Message);

        return Ok(Response.Data.Select(x => new ContainerJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/stacks/{stackId?}")]
    [SwaggerOperation("Get server stack info.", "Requires View Server and View Stack permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetStack([FromRoute] string serverId = "", [FromRoute] string stackId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(stackId))
            return BadRequest("Stack id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<DockerStackInfo?> Response = await server.RecieveJsonAsync<DockerStackInfo>(new DockerEvent(DockerEventType.ControlStack, stackId, stackType: ControlStackType.View));
        if (!Response.IsSuccess)
            return Conflict("Failed to get stack data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Stack does not exist");

        return Ok(new StackJson(Response.Data));
    }

    [HttpDelete("/api/servers/{serverId?}/stacks/{stackId?}/remove")]
    [SwaggerOperation("Delete a server stack.", "Requires View Server, View Stack and Manage Stack permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> RemoveStack([FromRoute] string serverId = "", [FromRoute] string stackId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(stackId))
            return BadRequest("Stack id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewStacks | DockerContainerPermission.ManageStacks, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<object?> Response = await server.RecieveJsonAsync<object>(new DockerEvent(DockerEventType.ControlStack, stackId, stackType: ControlStackType.Remove));
        if (!Response.IsSuccess)
            return Conflict("Failed to remove stack, " + Response.Message);

        return Ok();
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}")]
    [SwaggerOperation("Get server container info.", "Requires View Server and View Containers permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<StackJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetContainers([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<ContainerInspectResponse?> Response = await server.RecieveJsonAsync<ContainerInspectResponse>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Inspect));
        if (!Response.IsSuccess)
            return Conflict("Failed to get container data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Container does not exist");

        bool ViewDetails = Client.HasDockerContainerPermission(server.Team, server, DockerContainerPermission.ViewContainerDetails);
        bool ViewEnvironment = Client.HasDockerContainerPermission(server.Team, server, DockerContainerPermission.ViewContainerEnvironment);
        bool ViewHealthLog = Client.HasDockerContainerPermission(server.Team, server, DockerContainerPermission.ViewContainerHealthLogs);

        return Ok(new ContainerJson(Response.Data, ViewDetails, ViewEnvironment, ViewHealthLog));
    }

    [HttpPatch("/api/servers/{serverId?}/containers/{containerId?}/control")]
    [SwaggerOperation("Control server container state.", "Requires View Server, View Container and Control Container permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    [ListParameterSwagger("type", ["start", "stop", "restart", "pause", "unpause", "kill"])]
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
            case ControlContainerType.Kill:
            case ControlContainerType.Pause:
            case ControlContainerType.Restart:
            case ControlContainerType.Start:
            case ControlContainerType.Stop:
            case ControlContainerType.UnPause:
                break;
            default:
                return BadRequest("Invalid control type parameter.");
        }

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ControlContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<ContainerInspectResponse?> Response = await server.RecieveJsonAsync<ContainerInspectResponse>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: control));
        if (!Response.IsSuccess)
            return Conflict("Failed to control container, " + Response.Message);

        return Ok();
    }

    [HttpPost("/api/servers/{serverId?}/containers/{containerId?}/rename")]
    [SwaggerOperation("Rename a container.", "Requires View Server, View Container and Manage Container permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerLogs([FromRoute] string serverId = "", [FromRoute] string containerId = "", [FromQuery, Required] string name = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (string.IsNullOrEmpty(name))
            return BadRequest("Name parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ManageContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<object?> Response = await server.RecieveJsonAsync<object>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Rename)
        {
            Data = JObject.FromObject(new CreateContainerEvent
            {
                Name = name
            })
        });

        if (!Response.IsSuccess)
            return Conflict("Failed to rename container, " + Response.Message);

        return Ok();
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}/logs")]
    [SwaggerOperation("Get server container logs.", "Requires View Server, View Container and View Container Logs permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<string[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerLogs([FromRoute] string serverId = "", [FromRoute] string containerId = "", [FromQuery] int limit = 100, [FromQuery] bool split = false, [FromQuery] bool showTimestamp = false)
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (limit < 1)
            return BadRequest("Log limit needs to 1 or more.");

        if (limit > 1000)
            return BadRequest("Log limit can only be a max of 1000.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ViewContainerLogs, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<DockerContainerLogs?> Response = await server.RecieveJsonAsync<DockerContainerLogs>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Logs)
        {
            Data = JObject.FromObject(new ContainerLogsEvent
            {
                Limit = limit,
                ShowTimestamp = showTimestamp
            })
        });
        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get container logs, " + Response.Message);

        if (split)
            return Ok(Response.Data.Logs.Split(
                    ["\r\n", "\r", "\n"],
                    StringSplitOptions.RemoveEmptyEntries
                ));
        else
            return Ok(new[] { Response.Data.Logs });
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}/changes")]
    [SwaggerOperation("Get server container file system changes.", "Requires View Server, View Container and View Container Changes permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerChangeJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerChanges([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ViewContainerChanges, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<DockerContainerChanges?> Response = await server.RecieveJsonAsync<DockerContainerChanges>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Changes));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get container changes, " + Response.Message);

        return Ok(Response.Data.Changes.Select(x => new ContainerChangeJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}/stats")]
    [SwaggerOperation("Get server container stats.", "Requires View Server, View Container and View Container Stats permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerStatsJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerStats([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ViewContainerStats, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<DockerContainerStats?> Response = await server.RecieveJsonAsync<DockerContainerStats>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Stats));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get container stats, " + Response.Message);

        return Ok(new ContainerStatsJson(Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}/processes")]
    [SwaggerOperation("Get server container processes.", "Requires View Server, View Container and View Container Stats permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerProcessJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerProcesses([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ViewContainerStats, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<DockerContainerProcesses?> Response = await server.RecieveJsonAsync<DockerContainerProcesses>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Processes));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get container processes, " + Response.Message);


        return Ok(new ContainerProcessesJson(Response.Data).processes);
    }

    [HttpGet("/api/servers/{serverId?}/containers/{containerId?}/inspect")]
    [SwaggerOperation("Get server container full docker information.", "Requires View Server and Docker Administrator permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ContainerInspectResponse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> ContainerInspect([FromRoute] string serverId = "", [FromRoute] string containerId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.DockerAdministrator, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<ContainerInspectResponse?> Response = await server.RecieveJsonAsync<ContainerInspectResponse>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: ControlContainerType.Inspect));
        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get container details, " + Response.Message);

        return Ok(Response.Data);
    }

    [HttpDelete("/api/servers/{serverId?}/containers/{containerId?}/remove")]
    [SwaggerOperation("Delete a server container.", "Requires View Server, View Container and Manage Container permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseSuccess))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> RemoveContainer([FromRoute] string serverId = "", [FromRoute] string containerId = "", [FromQuery] bool force = false)
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(containerId))
            return BadRequest("Container id parameter is missing from path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        if (Client.CheckFailedDockerContainerPermissions(server, DockerContainerPermission.ViewContainers | DockerContainerPermission.ManageContainers, out DockerContainerPermission? dockerContainerPerm))
            return PermissionFailed(dockerContainerPerm!);

        SocketResponse<object?> Response = await server.RecieveJsonAsync<object>(new DockerEvent(DockerEventType.ControlContainer, containerId, containerType: force ? ControlContainerType.ForceRemove : ControlContainerType.Remove));
        if (!Response.IsSuccess)
            return Conflict("Failed to remove container, " + Response.Message);

        return Ok();
    }

    [HttpGet("/api/servers/{serverId?}/images")]
    [SwaggerOperation("Get server images list.", "Requires View Server and View Image permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ImageJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetImages([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewImages, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerImageInfo[]?> Response = await server.RecieveJsonAsync<DockerImageInfo[]>(new DockerEvent(DockerEventType.ListImages));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get images data, " + Response.Message);

        return Ok(Response.Data.Select(x => new ImageJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/images/{imageId?}")]
    [SwaggerOperation("Get a server image.", "Requires View Server and View Image permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ImageJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetImage([FromRoute] string serverId = "", [FromRoute] string imageId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(imageId))
            return BadRequest("Image id parameter is missing in path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewImages, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerImageInfo?> Response = await server.RecieveJsonAsync<DockerImageInfo>(new DockerEvent(DockerEventType.ControlImage, imageId, imageType: ControlImageType.View));

        if (!Response.IsSuccess)
            return Conflict("Failed to get image data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Image does not exist.");

        return Ok(new ImageJson(Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/volumes")]
    [SwaggerOperation("Get server volumes list.", "Requires View Server and View Volume permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<VolumeJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetVolumes([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewVolumes, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerVolumeInfo[]?> Response = await server.RecieveJsonAsync<DockerVolumeInfo[]>(new DockerEvent(DockerEventType.ListVolumes));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get volumes data, " + Response.Message);

        return Ok(Response.Data.Select(x => new VolumeJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/volumes/{volumeId?}")]
    [SwaggerOperation("Get a server volume.", "Requires View Server and View Volume permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<VolumeJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetVolume([FromRoute] string serverId = "", [FromRoute] string volumeId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(volumeId))
            return BadRequest("Volume id parameter is missing in route.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewVolumes, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerVolumeInfo?> Response = await server.RecieveJsonAsync<DockerVolumeInfo>(new DockerEvent(DockerEventType.ControlVolume, volumeId, volumeType: ControlVolumeType.View));

        if (!Response.IsSuccess)
            return Conflict("Failed to get volume data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Volume does not exist.");

        return Ok(new VolumeJson(Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/networks")]
    [SwaggerOperation("Get server networks list.", "Requires View Server and View Network permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<NetworkJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetNetworks([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewNetworks, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerNetworkInfo[]?> Response = await server.RecieveJsonAsync<DockerNetworkInfo[]>(new DockerEvent(DockerEventType.ListNetworks));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get networks data, " + Response.Message);

        return Ok(Response.Data.Select(x => new NetworkJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/networks/{networkId?}")]
    [SwaggerOperation("Get a server network.", "Requires View Server and View Network permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<NetworkJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetNetwork([FromRoute] string serverId = "", [FromRoute] string networkId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(networkId))
            return BadRequest("Network id parameter is missing in path.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewNetworks, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<DockerNetworkInfo?> Response = await server.RecieveJsonAsync<DockerNetworkInfo>(new DockerEvent(DockerEventType.ControlNetwork, networkId, networkType: ControlNetworkType.View));

        if (!Response.IsSuccess)
            return Conflict("Failed to get network data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Network does not exist.");

        return Ok(new NetworkJson(Response.Data));
    }

    [HttpGet("/api/servers/{serverId?}/plugins")]
    [SwaggerOperation("Get server plugins list.", "Requires View Server and View Plugin permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<PluginJson[]>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetPlugins([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewPlugins, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<Plugin[]?> Response = await server.RecieveJsonAsync<Plugin[]>(new DockerEvent(DockerEventType.ListPlugins));

        if (!Response.IsSuccess || Response.Data == null)
            return Conflict("Failed to get plugins data, " + Response.Message);

        return Ok(Response.Data.Select(x => new PluginJson(x)));
    }

    [HttpGet("/api/servers/{serverId?}/plugins/{pluginId?}")]
    [SwaggerOperation("Get a server plugins.", "Requires View Server and View Plugin permissions.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<PluginJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetPlugin([FromRoute] string serverId = "", [FromRoute] string pluginId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(server.TeamId == Client.TeamId))
            return NotFound("Could not find server.");

        if (string.IsNullOrEmpty(pluginId))
            return BadRequest("Plugin id parameter is missing in route.");

        if (Client.CheckFailedServerPermissions(server, ServerPermission.ViewServer, out ServerPermission? perm))
            return PermissionFailed(perm!);

        if (Client.CheckFailedDockerPermissions(server, DockerPermission.UseAPIs | DockerPermission.ViewPlugins, out DockerPermission? dockerPerm))
            return PermissionFailed(dockerPerm!);

        SocketResponse<Plugin?> Response = await server.RecieveJsonAsync<Plugin>(new DockerEvent(DockerEventType.ControlPlugin, pluginId, pluginType: ControlPluginType.Inspect));

        if (!Response.IsSuccess)
            return Conflict("Failed to get plugin data, " + Response.Message);

        if (Response.Data == null)
            return NotFound("Plugin does not exist.");

        return Ok(new PluginJson(Response.Data));
    }
}
