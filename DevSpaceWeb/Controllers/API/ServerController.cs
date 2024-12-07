using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
public class ServerController : Controller
{
    [HttpGet("/api/server/{serverId?}")]
    public async Task GetServer([FromRoute] string serverId = "")
    {

    }

    [HttpPost("/api/server/{serverId?}")]
    public async Task EditServer([FromRoute] string serverId = "")
    {

    }
}
