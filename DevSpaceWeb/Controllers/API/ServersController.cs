using DevSpaceWeb.API;
using DevSpaceWeb.API.Servers;
using DevSpaceWeb.API.Users;
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
public class ServersController : APIController
{
    [HttpGet("/api/servers/{serverId?}")]
    [SwaggerOperation("Get a server.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ServerJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetServer([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? server) || !(Client.IsInstanceAdmin || server.TeamId == Client.TeamId.GetValueOrDefault()))
            return BadRequest("Could not find server.");

        if (!Client.HasServerPermission(server, ServerPermission.ViewServer))
            return Forbidden("Client does not have View Server permission.");

        return Ok(new ServerJson(server));
    }
}
