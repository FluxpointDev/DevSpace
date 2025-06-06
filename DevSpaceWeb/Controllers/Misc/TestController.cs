﻿using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb.Controllers.Misc;
public class TestController : Controller
{
    [HttpGet("/api/test/websocket")]
    public async Task<IActionResult> TestWebsocket()
    {
        string? Connection = Request.Headers["Connection"];
        return Connection == "close" ? StatusCode(412) : Ok();
    }

    [HttpGet("/api/test/headers")]
    public async Task<IActionResult> Headers()
    {
        if (Program.IsPreviewMode)
            return BadRequest("Preview mode is enabled.");

        Logger.LogMessage("---", LogSeverity.Debug);
        foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> i in Request.Headers)
        {
            Logger.LogMessage($"Header: {i.Key} - {i.Value.ToString()}", LogSeverity.Debug);
        }
        Logger.LogMessage("---", LogSeverity.Debug);
        return Ok(Request.Headers);
    }

}
