using DevSpaceWeb.API.Servers;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
public class ServerController : Controller
{
    [HttpGet("/api/servers/{serverId?}")]
    public async Task<IActionResult> GetServer([FromRoute] string serverId = "")
    {
        if (string.IsNullOrEmpty(serverId) || !ObjectId.TryParse(serverId, out var obj) || !_DB.Servers.Cache.TryGetValue(obj, out var Server))
            return BadRequest("Could not find server.");

        return Ok(new ServerJson(Server));
    }
}
