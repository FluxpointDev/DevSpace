using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Apps.Runtime.DiscordApp;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace DevSpaceWeb.Apps;

public class WorkspaceController : ControllerBase
{
    public WorkspaceController(UserManager<AuthUser> userManager)
    {
        UserManager = userManager;
    }
    private readonly UserManager<AuthUser> UserManager;

    [HttpGet("/dev/workspace/toolbox")]
    public async Task<IActionResult> FetchWorkspaceToolbox()
    {
        string? WorkspaceId = Request.Query["id"];
        if (string.IsNullOrEmpty(WorkspaceId))
        {
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ToolboxDef
            {
                toolbox = _Data.WorkspaceData.Discord.ToolboxItem,
                type = 0
            }, Newtonsoft.Json.Formatting.Indented));
        }

        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("You are not logged in.");

        if (string.IsNullOrEmpty(WorkspaceId) || !ObjectId.TryParse(WorkspaceId, out ObjectId workspaceId))
            return BadRequest("Invalid workspace id..");

        AuthUser? AuthUser = await UserManager.GetUserAsync(User);
        if (AuthUser == null)
            return Unauthorized("Invalid user account.");

        WorkspaceData? Workspace = _DB.Workspaces.Collection.Find(new FilterDefinitionBuilder<WorkspaceData>().Eq(x => x.Id, workspaceId)).FirstOrDefault();
        if (Workspace == null)
            return BadRequest("Could not find workspace.");

        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new ToolboxDef
        {
            toolbox = _Data.WorkspaceData.Discord.ToolboxItem,
            type = (int)Workspace.Type
        }, Newtonsoft.Json.Formatting.Indented));
    }

    [HttpGet("/dev/workspace/fetch")]
    public async Task<IActionResult> FetchWorkspace()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("You are not logged in.");

        string? WorkspaceId = Request.Query["id"];
        if (string.IsNullOrEmpty(WorkspaceId) || !ObjectId.TryParse(WorkspaceId, out ObjectId workspaceId))
            return BadRequest("Invalid workspace id..");

        AuthUser? AuthUser = await UserManager.GetUserAsync(User);
        if (AuthUser == null)
            return Unauthorized("Invalid user account.");

        WorkspaceData? Workspace = _DB.Workspaces.Collection.Find(new FilterDefinitionBuilder<WorkspaceData>().Eq(x => x.Id, workspaceId)).FirstOrDefault();
        if (Workspace == null)
            return BadRequest("Could not find workspace.");

        if (!_DB.Apps.Cache.TryGetValue(Workspace.AppId, out AppData? app))
            return BadRequest("Could not find app.");

        if (!_DB.Teams.Cache.TryGetValue(app.TeamId, out TeamData? team))
            return BadRequest("Could not find team.");

        TeamMemberData? Member = team.GetMember(AuthUser);
        if (Member == null || !Member.HasAppPermission(team, app, AppPermission.ViewWorkspaces))
            return BadRequest("You do not have permission.");

        if (string.IsNullOrEmpty(Workspace.JsonData))
        {
            switch (Workspace.Type)
            {
                case WorkspaceType.DiscordSlashCommand:
                    {
                        return Content(@"{ ""blocks"": { ""blocks"": [
                                     { 
                                        ""type"": ""block_command"", ""x"": 380, ""y"": 160,
                                        ""fields"": { ""name"": """ + Workspace.CommandFormat + @""" }
                                    }
                                    ] } }", "application/json");
                    }
                case WorkspaceType.DiscordUserCommand:
                    {
                        return Content(@"{ ""blocks"": { ""blocks"": [
                                     { 
                                        ""type"": ""block_user_command"", ""x"": 380, ""y"": 160,
                                        ""fields"": { ""name"": """ + Workspace.CommandFormat + @""" }
                                    }
                                    ] } }", "application/json");
                    }
                case WorkspaceType.DiscordMessageCommand:
                    {
                        return Content(@"{ ""blocks"": { ""blocks"": [
                                     { 
                                        ""type"": ""block_message_command"", ""x"": 380, ""y"": 160,
                                        ""fields"": { ""name"": """ + Workspace.CommandFormat + @""" }
                                    }
                                    ] } }", "application/json");
                    }
                case WorkspaceType.DiscordInteractionButton:
                    {
                        return Content(@"{ ""blocks"": { ""blocks"": [
                                     { 
                                        ""type"": ""block_interaction_button"", ""x"": 380, ""y"": 160,
                                        ""fields"": { ""name"": """ + Workspace.CommandFormat + @""" }
                                    }
                                    ] } }", "application/json");
                    }
                case WorkspaceType.DiscordInteractionModal:
                    {
                        return Content(@"{ ""blocks"": { ""blocks"": [
                                     { 
                                        ""type"": ""block_interaction_modal"", ""x"": 380, ""y"": 160,
                                        ""fields"": { ""name"": """ + Workspace.CommandFormat + @""" },
                                        ""inputs"": { ""modal_title"": { ""block"": { ""type"": ""text"" }  } }
                                    }
                                    ] } }", "application/json");
                    }
            }
        }

        return Content(Workspace.JsonData, "application/json");
    }

    [HttpGet("/workspaces/{app_id?}/{workspace_id?}/share/fetch")]
    public async Task<IActionResult> FetchSharedWorkspace([FromRoute] string? app_id = null, [FromRoute] string? workspace_id = null)
    {
        if (string.IsNullOrEmpty(workspace_id) || !ObjectId.TryParse(workspace_id, out ObjectId workspaceId))
            return BadRequest("Invalid workspace id..");

        if (string.IsNullOrEmpty(app_id) || !ObjectId.TryParse(app_id, out ObjectId appId))
            return BadRequest("Could not find app.");

        if (!_DB.Apps.Cache.ContainsKey(appId))
            return BadRequest("Could not find app.");

        WorkspaceData? Workspace = _DB.Workspaces.Collection.Find(new FilterDefinitionBuilder<WorkspaceData>().Eq(x => x.Id, workspaceId)).FirstOrDefault();
        if (Workspace == null || !Workspace.IsPublic)
            return BadRequest("Could not find workspace.");

        return Content(Workspace.JsonData, "application/json");
    }

    [HttpPost("/dev/workspace/update")]
    public async Task<IActionResult> SaveWorkspace()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized("You are not logged in.");

        string? WorkspaceId = Request.Query["id"];
        if (string.IsNullOrEmpty(WorkspaceId) || !ObjectId.TryParse(WorkspaceId, out ObjectId workspaceId))
            return BadRequest("Invalid workspace id..");

        AuthUser? AuthUser = await UserManager.GetUserAsync(User);
        if (AuthUser == null)
            return Unauthorized("Invalid user account.");

        WorkspaceData? Workspace = _DB.Workspaces.Collection.Find(new FilterDefinitionBuilder<WorkspaceData>().Eq(x => x.Id, workspaceId)).FirstOrDefault();
        if (Workspace == null)
            return BadRequest("Could not find workspace.");

        if (!_DB.Apps.Cache.TryGetValue(Workspace.AppId, out AppData? app))
            return BadRequest("Could not find app.");

        if (!_DB.Teams.Cache.TryGetValue(app.TeamId, out TeamData? team))
            return BadRequest("Could not find team.");

        TeamMemberData? Member = team.GetMember(AuthUser);
        if (Member == null || !Member.HasAppPermission(team, app, AppPermission.ManageWorkspaces))
            return BadRequest("You do not have permission.");

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
            return BadRequest("Request body does not have json content.");
        }
        WorkspaceRequest? Command = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceRequest>(body, new Newtonsoft.Json.JsonSerializerSettings
        {
            MaxDepth = 512
        });

        if (Command == null)
            return BadRequest("Failed to parse command data.");

        await Workspace.UpdateAsync(new UpdateDefinitionBuilder<WorkspaceData>().Set(x => x.JsonData, Newtonsoft.Json.JsonConvert.SerializeObject(Command, new Newtonsoft.Json.JsonSerializerSettings
        {
            MaxDepth = 512
        })));

        app.GetCache().AddCommandCache(Workspace.Id, Command.blocks.blocks[0]);

        bool HasEphemeralResponse = false;
        bool HasUserAppsResponse = false;
        Tuple<string, string>? OpenModal = null;
        Command.blocks.blocks.First().inputs.TryGetValue("command_options", out WorkspaceBlockConnection? optBlocks);
        if (optBlocks != null && optBlocks.block != null)
        {
            HasEphemeralResponse = HasEphemeral(optBlocks.block);
            HasUserAppsResponse = HasUserApps(optBlocks.block);
            OpenModal = HasOpenModal(optBlocks.block);
        }

        string Name = Workspace.CommandFormat;

        switch (Workspace.Type)
        {
            case WorkspaceType.DiscordSlashCommand:
                {

                    DiscordAppSlashCommand? CommandData = null;
                    DiscordAppSlashCommand? FirstData = null;
                    string[] CommandId = Name.Split(' ');
                    if (!string.IsNullOrEmpty(CommandId[0]))
                    {
                        if (string.IsNullOrEmpty(Workspace.ServerId))
                            app.SlashCommands.TryGetValue(CommandId[0], out CommandData);
                        else if (app.ServerCommands.TryGetValue(CommandId[0], out DiscordAppServerCommands? sc))
                            sc.SlashCommands.TryGetValue(CommandId[0], out CommandData);

                        if (CommandData != null)
                        {
                            FirstData = CommandData;
                            if (CommandData.WorkspaceId != Workspace.Id)
                            {
                                if (CommandId.Length == 3)
                                {
                                    if (CommandData.SubGroups.TryGetValue(CommandId[1], out AppSubGroup? sub))
                                        sub.Commands.TryGetValue(CommandId[2], out CommandData);


                                }
                                else if (CommandId.Length == 2)
                                {
                                    CommandData.Commands.TryGetValue(CommandId[1], out CommandData);
                                }
                            }
                        }
                    }

                    if (CommandData == null)
                        return BadRequest("Failed, command data invalid.");



                    string InputHash = string.Empty;
                    List<DiscordAppCommandInput> Inputs = new List<DiscordAppCommandInput>();

                    if (Command.blocks.blocks.First().inputs.TryGetValue("command_inputs", out WorkspaceBlockConnection? inputBlocks) && inputBlocks.block != null)
                    {
                        Inputs = await DiscordInputs.Parse(inputBlocks.block);
                        if (Inputs.Count != 0)
                            InputHash = GetHashString(Newtonsoft.Json.JsonConvert.SerializeObject(Inputs));

                    }

                    bool CanSave = true;
                    CommandData.UpdatedAt = DateTime.UtcNow;
                    CommandData.UpdatedBy = AuthUser.Id;
                    if (OpenModal == null)
                    {
                        if (!string.IsNullOrEmpty(CommandData.OpenModal))
                        {
                            CommandData.OpenModal = string.Empty;
                            CommandData.ModalDataType = string.Empty;
                            CanSave = true;
                        }
                    }
                    else
                    {
                        if (CommandData.OpenModal != OpenModal.Item1)
                        {
                            CommandData.OpenModal = OpenModal.Item1;
                            CanSave = true;
                        }
                        if (CommandData.ModalDataType != OpenModal.Item2)
                        {
                            CommandData.ModalDataType = OpenModal.Item2;
                            CanSave = true;
                        }
                    }


                    if (CommandData.IsEphemeral != HasEphemeralResponse)
                    {
                        CommandData.IsEphemeral = HasEphemeralResponse;
                        CanSave = true;
                    }
                    bool ChangeContext = false;
                    if (CommandData.AllowUserApps != HasUserAppsResponse)
                    {
                        ChangeContext = true;
                        CommandData.AllowUserApps = HasUserAppsResponse;
                        CanSave = true;
                    }

                    if (CommandData.InputHash != InputHash || ChangeContext)
                    {
                        bool ChangeInputs = CommandData.InputHash != InputHash;
                        CommandData.InputHash = InputHash;
                        CommandData.Inputs = Inputs.ToArray();
                        CanSave = true;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                if (_Data.DiscordClients.TryGetValue(app.Id, out DiscordRestClient? client))
                                {
                                    IReadOnlyCollection<RestApplicationCommand> DiscordCommands = string.IsNullOrEmpty(Workspace.ServerId) ? await client.GetGlobalApplicationCommands() : await client.GetGuildApplicationCommands(ulong.Parse(Workspace.ServerId));
                                    RestApplicationCommand? DiscordCommand = DiscordCommands.FirstOrDefault(x => x.Id.ToString() == FirstData.CommandId);
                                    if (DiscordCommand != null)
                                    {
                                        await DiscordCommand.ModifyAsync<SlashCommandProperties>(x =>
                                        {
                                            if (ChangeInputs)
                                                x.Options = CommandData.Build().Options;

                                            x.ContextTypes = new HashSet<InteractionContextType> { InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel };

                                            if (ChangeContext)
                                                x.IntegrationTypes = HasUserAppsResponse ? new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall } : new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall };

                                        });

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    }

                    if (CanSave)
                    {
                        await app.UpdateAsync(new UpdateDefinitionBuilder<AppData>().Set(x => x.SlashCommands, app.SlashCommands));
                    }
                }
                break;
            case WorkspaceType.DiscordUserCommand:
                {
                    Name = Workspace.CommandFormat;

                    IDiscordAppCommand? cmd = null;
                    if (string.IsNullOrEmpty(Workspace.ServerId))
                        app.UserCommands.TryGetValue(Name.ToLower(), out cmd);
                    else if (app.ServerCommands.TryGetValue(Workspace.ServerId, out DiscordAppServerCommands? sc))
                        sc.UserCommands.TryGetValue(Name.ToLower(), out cmd);

                    if (cmd == null)
                        return BadRequest("Failed, command data invalid.");


                    bool CanSave = true;
                    cmd.UpdatedAt = DateTime.UtcNow;
                    cmd.UpdatedBy = AuthUser.Id;
                    if (OpenModal == null)
                    {
                        if (!string.IsNullOrEmpty(cmd.OpenModal))
                        {
                            cmd.OpenModal = string.Empty;
                            cmd.ModalDataType = string.Empty;
                            CanSave = true;
                        }
                    }
                    else
                    {
                        if (cmd.OpenModal != OpenModal.Item1)
                        {
                            cmd.OpenModal = OpenModal.Item1;
                            CanSave = true;
                        }
                        if (cmd.ModalDataType != OpenModal.Item2)
                        {
                            cmd.ModalDataType = OpenModal.Item2;
                            CanSave = true;
                        }
                    }
                    if (cmd.IsEphemeral != HasEphemeralResponse)
                    {
                        cmd.IsEphemeral = HasEphemeralResponse;
                        CanSave = true;
                    }
                    bool ChangeContext = false;
                    if (cmd.AllowUserApps != HasUserAppsResponse)
                    {
                        ChangeContext = true;
                        cmd.AllowUserApps = HasUserAppsResponse;
                        CanSave = true;
                    }

                    if (ChangeContext)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                if (_Data.DiscordClients.TryGetValue(app.Id, out DiscordRestClient? client))
                                {
                                    IReadOnlyCollection<RestApplicationCommand> DiscordCommands = string.IsNullOrEmpty(Workspace.ServerId) ? await client.GetGlobalApplicationCommands() : await client.GetGuildApplicationCommands(ulong.Parse(Workspace.ServerId));
                                    RestApplicationCommand? DiscordCommand = DiscordCommands.FirstOrDefault(x => x.Id.ToString() == cmd.CommandId);
                                    if (DiscordCommand != null)
                                    {
                                        await DiscordCommand.ModifyAsync<UserCommandProperties>(x =>
                                        {
                                            x.ContextTypes = new HashSet<InteractionContextType> { InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel };

                                            if (ChangeContext)
                                                x.IntegrationTypes = HasUserAppsResponse ? new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall } : new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall };

                                        });

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    }

                    if (CanSave)
                    {
                        await app.UpdateAsync(new UpdateDefinitionBuilder<AppData>().Set(x => x.UserCommands, app.UserCommands));
                    }
                }
                break;
            case WorkspaceType.DiscordMessageCommand:
                {
                    if (string.IsNullOrEmpty(Name))
                        return BadRequest("Empty name");
                    Name = Name.ToLower();

                    IDiscordAppCommand? cmd = null;
                    if (string.IsNullOrEmpty(Workspace.ServerId))
                        app.MessageCommands.TryGetValue(Name.ToLower(), out cmd);
                    else if (app.ServerCommands.TryGetValue(Workspace.ServerId, out DiscordAppServerCommands? sc))
                        sc.MessageCommands.TryGetValue(Name.ToLower(), out cmd);

                    if (cmd == null)
                        return BadRequest("Failed, command data invalid.");

                    bool CanSave = true;
                    cmd.UpdatedAt = DateTime.UtcNow;
                    cmd.UpdatedBy = AuthUser.Id;
                    if (OpenModal == null)
                    {
                        if (!string.IsNullOrEmpty(cmd.OpenModal))
                        {
                            cmd.OpenModal = string.Empty;
                            cmd.ModalDataType = string.Empty;
                            CanSave = true;
                        }
                    }
                    else
                    {
                        if (cmd.OpenModal != OpenModal.Item1)
                        {
                            cmd.OpenModal = OpenModal.Item1;
                            CanSave = true;
                        }
                        if (cmd.ModalDataType != OpenModal.Item2)
                        {
                            cmd.ModalDataType = OpenModal.Item2;
                            CanSave = true;
                        }
                    }
                    if (cmd.IsEphemeral != HasEphemeralResponse)
                    {
                        cmd.IsEphemeral = HasEphemeralResponse;
                        CanSave = true;
                    }
                    bool ChangeContext = false;
                    if (cmd.AllowUserApps != HasUserAppsResponse)
                    {
                        ChangeContext = true;
                        cmd.AllowUserApps = HasUserAppsResponse;
                        CanSave = true;
                    }

                    if (ChangeContext)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                if (_Data.DiscordClients.TryGetValue(app.Id, out DiscordRestClient? client))
                                {
                                    IReadOnlyCollection<RestApplicationCommand> DiscordCommands = string.IsNullOrEmpty(Workspace.ServerId) ? await client.GetGlobalApplicationCommands() : await client.GetGuildApplicationCommands(ulong.Parse(Workspace.ServerId));
                                    RestApplicationCommand? DiscordCommand = DiscordCommands.FirstOrDefault(x => x.Id.ToString() == cmd.CommandId);
                                    if (DiscordCommand != null)
                                    {
                                        await DiscordCommand.ModifyAsync<UserCommandProperties>(x =>
                                        {
                                            x.ContextTypes = new HashSet<InteractionContextType> { InteractionContextType.Guild, InteractionContextType.BotDm, InteractionContextType.PrivateChannel };

                                            if (ChangeContext)
                                                x.IntegrationTypes = HasUserAppsResponse ? new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall } : new HashSet<ApplicationIntegrationType> { ApplicationIntegrationType.GuildInstall };

                                        });

                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    }

                    if (CanSave)
                    {
                        await app.UpdateAsync(new UpdateDefinitionBuilder<AppData>().Set(x => x.MessageCommands, app.MessageCommands));
                    }
                }
                break;
            case WorkspaceType.DiscordInteractionButton:
                {
                    app.ButtonInteractions.TryGetValue(Workspace.CommandFormat, out IDiscordAppInteraction? cmd);

                    if (cmd == null)
                        return BadRequest("Failed, command data invalid.");

                    bool CanSave = true;
                    cmd.UpdatedAt = DateTime.UtcNow;
                    cmd.UpdatedBy = AuthUser.Id;
                    if (OpenModal == null)
                    {
                        if (!string.IsNullOrEmpty(cmd.OpenModal))
                        {
                            cmd.OpenModal = string.Empty;
                            cmd.ModalDataType = string.Empty;
                            CanSave = true;
                        }
                    }
                    else
                    {
                        if (cmd.OpenModal != OpenModal.Item1)
                        {
                            cmd.OpenModal = OpenModal.Item1;
                            CanSave = true;
                        }
                        if (cmd.ModalDataType != OpenModal.Item2)
                        {
                            cmd.ModalDataType = OpenModal.Item2;
                            CanSave = true;
                        }
                    }
                    if (cmd.IsEphemeral != HasEphemeralResponse)
                    {
                        cmd.IsEphemeral = HasEphemeralResponse;
                        CanSave = true;
                    }

                    if (CanSave)
                        await app.UpdateAsync(new UpdateDefinitionBuilder<AppData>().Set(x => x.ButtonInteractions, app.ButtonInteractions));
                }
                break;
            case WorkspaceType.DiscordInteractionModal:
                {
                    app.ModalInteractions.TryGetValue(Workspace.CommandFormat, out IDiscordAppInteraction? cmd);

                    if (cmd == null)
                        return BadRequest("Failed, command data invalid.");

                    bool CanSave = true;
                    cmd.UpdatedAt = DateTime.UtcNow;
                    cmd.UpdatedBy = AuthUser.Id;
                    if (OpenModal == null)
                    {
                        if (!string.IsNullOrEmpty(cmd.OpenModal))
                        {
                            cmd.OpenModal = string.Empty;
                            cmd.ModalDataType = string.Empty;
                            CanSave = true;
                        }
                    }
                    else
                    {
                        if (cmd.OpenModal != OpenModal.Item1)
                        {
                            cmd.OpenModal = OpenModal.Item1;
                            CanSave = true;
                        }
                        if (cmd.ModalDataType != OpenModal.Item2)
                        {
                            cmd.ModalDataType = OpenModal.Item2;
                            CanSave = true;
                        }
                    }
                    if (cmd.IsEphemeral != HasEphemeralResponse)
                    {
                        cmd.IsEphemeral = HasEphemeralResponse;
                        CanSave = true;
                    }

                    if (CanSave)
                        await app.UpdateAsync(new UpdateDefinitionBuilder<AppData>().Set(x => x.ModalInteractions, app.ModalInteractions));
                }
                break;
        }

        return Ok();
    }

    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }


    public bool HasEphemeral(WorkspaceBlock block)
    {
        if (block.enabled && block.type == "option_ephemeral")
            return true;

        if (block.next != null && block.next.block != null)
        {
            bool test = HasEphemeral(block.next.block);
            if (test)
                return test;
        }

        return false;
    }

    public Tuple<string, string>? HasOpenModal(WorkspaceBlock block)
    {
        if (block.enabled && block.type == "option_open_modal")
        {
            if (block.inputs.TryGetValue("data", out WorkspaceBlockConnection? dbBlock) && dbBlock.block != null)
                return Tuple.Create(block.inputs["name"].block.fields["TEXT"].ToString(), dbBlock.block.type == "variables_get" ? dbBlock.block.fields["VAR"]["id"].ToString() : dbBlock.block.type);

            return Tuple.Create(block.inputs["name"].block.fields["TEXT"].ToString(), string.Empty);
        }


        if (block.next != null && block.next.block != null)
        {
            Tuple<string, string>? test = HasOpenModal(block.next.block);
            if (test != null)
                return test;
        }

        return null;
    }

    public bool HasUserApps(WorkspaceBlock block)
    {
        if (block.enabled && block.type == "option_allow_user_apps")
            return true;

        if (block.next != null && block.next.block != null)
        {
            bool test = HasUserApps(block.next.block);
            if (test)
                return test;
        }

        return false;
    }

    public class ToolboxDef
    {
        public int type;
        public BlocklyToolboxItem toolbox;
    }

    public class WorkspaceConfig
    {
        public DiscordEnvironment Discord = new DiscordEnvironment();
    }

    public class DiscordEnvironment
    {
        public BlocklyToolboxItem ToolboxItem = new BlocklyToolboxItem();
    }
}
