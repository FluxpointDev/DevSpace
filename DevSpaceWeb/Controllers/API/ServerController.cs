using DevSpaceWeb.API.Servers;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
public class ServerController : APIController
{
    [HttpGet("/api/servers/{serverId?}")]
    public async Task<IActionResult> GetServer([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out ObjectId obj) || !_DB.Servers.Cache.TryGetValue(obj, out Data.Servers.ServerData? Server))
            return BadRequest("Could not find server.");

        return Ok(new ServerJson(Server));
    }
}
