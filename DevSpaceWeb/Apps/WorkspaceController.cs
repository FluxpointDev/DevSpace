using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace DevSpaceWeb.Apps;

public class WorkspaceController : ControllerBase
{
    [HttpGet("/dev/workspace/toolbox/fetch")]
    public async Task<IActionResult> FetchWorkspaceToolbox()
    {
        string? Id = Request.Query["id"];

        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ToolboxDef
        {
            toolbox = _Data.WorkspaceData.DevEnvironment.Discord.ToolboxItem,
            type = 0
        }, Newtonsoft.Json.Formatting.Indented));

    }

    [HttpGet("/dev/workspace/fetch")]
    public async Task<IActionResult> FetchWorkspace()
    {
        string? Id = Request.Query["id"];

        return Content("{}");
    }

    [HttpPost("/dev/workspace")]
    public async Task<IActionResult> SaveWorkspace()
    {
        if (!Program.IsDevMode)
            return NotFound();

        string body = "";

        try
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Request body does not have json content. ( CG-1 )");
        }
        RequestBlocks? Command = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestBlocks>(body, new Newtonsoft.Json.JsonSerializerSettings
        {
            MaxDepth = 256
        });

        Data.AppData App = _DB.Apps.Cache.First().Value;
        App.GetCache().AddCommandCache(ObjectId.Parse("685f25592310c4de13f20380"), Command.blocks.blocks[0]);

        return Ok();
    }

    public class ToolboxDef
    {
        public int type;
        public BlocklyToolboxItem toolbox;
    }

    public class WorkspaceConfig
    {
        public GlobalEnvironment PublicEnvironment = new GlobalEnvironment();
        public GlobalEnvironment DevEnvironment = new GlobalEnvironment();
    }

    public class GlobalEnvironment
    {
        public DiscordEnvironment Discord = new DiscordEnvironment();
    }
    public class DiscordEnvironment
    {
        public BlocklyToolboxItem ToolboxItem = new BlocklyToolboxItem();
    }
}
