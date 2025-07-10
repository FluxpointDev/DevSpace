using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Apps.Runtime;
using DevSpaceWeb.Apps.Runtime.DiscordApp;
using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using Discord;
using Discord.Net.Converters;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace DevSpaceWeb.Apps;
public class RunController : Controller
{
    public static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

    public async Task HttpRespondWithError(HttpRequest request, RestInteraction interaction, string error, CancellationToken cancellationToken)
    {
        request.HttpContext.Response.Headers.Append("Content-Type", "application/json");
        await request.HttpContext.Response.WriteAsync(interaction.Respond(error, ephemeral: true), cancellationToken);
        await request.HttpContext.Response.CompleteAsync();
    }
    public async Task HttpResponseWithModal(HttpRequest request, RestInteraction interaction, Modal modal, CancellationToken cancellationToken)
    {
        request.HttpContext.Response.Headers.Append("Content-Type", "application/json");
        await request.HttpContext.Response.WriteAsync(interaction.RespondWithModal(modal), cancellationToken);
        await request.HttpContext.Response.CompleteAsync();
    }

    [HttpPost("/interactions/{app_id}")]
    public async Task InteractionsAsync(CancellationToken cancellationToken, string app_id)
    {
        if (string.IsNullOrEmpty(app_id) || app_id.Length > 60)
            return;

        DateTime currentTime = DateTime.Now;

        if (!ObjectId.TryParse(app_id, out ObjectId id))
            return;

        if (_DB.Apps.Cache.TryGetValue(id, out AppData? AppData) && _Data.DiscordClients.TryGetValue(id, out DiscordRestClient? Client))
        {
            string Decrypted = AppData.GetDecryptedInteractionKey();

            string? signature = Request.Headers["X-Signature-Ed25519"].FirstOrDefault();
            string? timestamp = Request.Headers["X-Signature-Timestamp"].FirstOrDefault();
            string body = "";
            using (StreamReader reader = new StreamReader(Request.Body))
                body = await reader.ReadToEndAsync();

            if (Client.LoginState != LoginState.LoggedIn)
            {
                Request.HttpContext.Response.StatusCode = 500;
                await Request.HttpContext.Response.WriteAsync("error");
                await Request.HttpContext.Response.CompleteAsync();
                return;
            }


            RestInteraction? Interaction = null;
            try
            {
                Interaction = await Client.ParseHttpInteractionAsync(Decrypted, signature, timestamp, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Request.HttpContext.Response.StatusCode = 401;
                await Request.HttpContext.Response.WriteAsync("invalid request signature");
                await Request.HttpContext.Response.CompleteAsync();
                return;
            }

            IDiscordAppInteraction? InteractionData = null;
            WorkspaceType Type = WorkspaceType.DiscordSlashCommand;
            string GuildId = ".";
            string outputDataType = string.Empty;
            if (Interaction.GuildId.HasValue)
                GuildId = Interaction.GuildId.ToString();

            switch (Interaction)
            {
                case RestPingInteraction Ping:
                    {
                        Request.HttpContext.Response.Headers.Append("Content-Type", "application/json");
                        await Request.HttpContext.Response.WriteAsync(Ping.AcknowledgePing(), cancellationToken);
                        await Request.HttpContext.Response.CompleteAsync();
                    }
                    return;
                case RestSlashCommand Cmd:
                    {
                        AppData.GetEvents().TriggerEvent(new AppLogItem()
                        {
                            Title = $"Slash Command: {Interaction.User.Username} ran {Cmd.Data.Name} in #{Interaction.Channel.Name}",
                            Data = body,
                            Type = WorkspaceType.DiscordSlashCommand
                        });

                        DiscordAppSlashCommand? SelectedCommand = null;

                        if (Cmd.Data.Options.Count != 0)
                        {
                            RestSlashCommandDataOption? First = Cmd.Data.Options.FirstOrDefault();
                            if (First != null)
                            {
                                if (First.Type == ApplicationCommandOptionType.SubCommand)
                                {
                                    if (AppData.SlashCommands.TryGetValue(Cmd.Data.Name, out DiscordAppSlashCommand? ac))
                                        ac.Commands.TryGetValue(First.Name, out SelectedCommand);

                                    if (SelectedCommand == null && Interaction.GuildId.HasValue && AppData.ServerCommands.TryGetValue(Interaction.GuildId.ToString(), out DiscordAppServerCommands? sc))
                                        sc.SlashCommands.TryGetValue(First.Name, out SelectedCommand);

                                }
                                else if (First.Type == ApplicationCommandOptionType.SubCommandGroup)
                                {
                                    RestSlashCommandDataOption? Next = First.Options.FirstOrDefault();
                                    if (Next != null && Next.Type == ApplicationCommandOptionType.SubCommand)
                                    {
                                        if (AppData.SlashCommands.TryGetValue(Cmd.Data.Name, out DiscordAppSlashCommand? ac) && ac.SubGroups.TryGetValue(First.Name, out AppSubGroup? sub))
                                            sub.Commands.TryGetValue(Next.Name, out SelectedCommand);

                                        if (SelectedCommand == null && Interaction.GuildId.HasValue && AppData.ServerCommands.TryGetValue(GuildId, out DiscordAppServerCommands? sc))
                                        {
                                            if (sc.SlashCommands.TryGetValue(Cmd.Data.Name, out ac) && ac.SubGroups.TryGetValue(First.Name, out sub))
                                                sub.Commands.TryGetValue(Next.Name, out SelectedCommand);
                                        }
                                    }
                                }
                            }
                        }
                        if (SelectedCommand == null)
                        {
                            SelectedCommand = AppData.SlashCommands.Values.FirstOrDefault(x => x.CommandId == Cmd.Data.Id.ToString());
                            if (SelectedCommand == null && Interaction.GuildId.HasValue && AppData.ServerCommands.TryGetValue(GuildId, out DiscordAppServerCommands? sc))
                            {
                                SelectedCommand = sc.SlashCommands.Values.FirstOrDefault(x => x.CommandId == Cmd.Data.Id.ToString());
                            }
                        }

                        InteractionData = SelectedCommand;
                    }
                    break;
                case RestUserCommand ucmd:
                    {
                        AppData.GetEvents().TriggerEvent(new AppLogItem()
                        {
                            Title = $"User Command: {Interaction.User.Username} ran {ucmd.Data.Name} in #{Interaction.Channel.Name}",
                            Data = body,
                            Type = WorkspaceType.DiscordUserCommand
                        });

                        AppData.UserCommands.TryGetValue(ucmd.Data.Name.ToLower(), out IDiscordAppCommand? command);
                        if (command == null && Interaction.GuildId.HasValue && AppData.ServerCommands.TryGetValue(GuildId, out DiscordAppServerCommands? sc))
                            sc.UserCommands.TryGetValue(ucmd.Data.Name.ToLower(), out command);

                        InteractionData = command;
                        Type = WorkspaceType.DiscordUserCommand;
                    }
                    break;
                case RestMessageCommand mcmd:
                    {
                        AppData.GetEvents().TriggerEvent(new AppLogItem()
                        {
                            Title = $"Message Command: {Interaction.User.Username} ran {mcmd.Data.Name} in #{Interaction.Channel.Name}",
                            Data = body,
                            Type = WorkspaceType.DiscordMessageCommand
                        });

                        AppData.MessageCommands.TryGetValue(mcmd.Data.Name.ToLower(), out IDiscordAppCommand? command);
                        if (command == null && Interaction.GuildId.HasValue && AppData.ServerCommands.TryGetValue(GuildId, out DiscordAppServerCommands? sc))
                            sc.MessageCommands.TryGetValue(mcmd.Data.Name.ToLower(), out command);

                        InteractionData = command;
                        Type = WorkspaceType.DiscordMessageCommand;
                    }
                    break;
                case RestMessageComponent ci:
                    {

                        switch (ci.Data.Type)
                        {
                            case ComponentType.Button:
                                {
                                    AppData.GetEvents().TriggerEvent(new AppLogItem()
                                    {
                                        Title = $"Button Interaction: {Interaction.User.Username} ran {ci.Id} in #{Interaction.Channel.Name}",
                                        Data = body,
                                        Type = WorkspaceType.DiscordInteractionButton
                                    });

                                    string[] SplitId = ci.Data.CustomId.Split('|', StringSplitOptions.RemoveEmptyEntries);
                                    string Id = SplitId[0];

                                    if (AppData.ButtonInteractions.TryGetValue(Id.Substring(2), out IDiscordAppInteraction? bi))
                                        InteractionData = bi;

                                    if (SplitId.Length == 2)
                                        outputDataType = SplitId[1];

                                    Type = WorkspaceType.DiscordInteractionButton;
                                }
                                break;
                            case ComponentType.TextInput:
                                {
                                    await HttpRespondWithError(Request, Interaction, "Text input is not supported with Nova Node yet.", cancellationToken);
                                    return;
                                }
                                break;
                            case ComponentType.ChannelSelect:
                            case ComponentType.MentionableSelect:
                            case ComponentType.RoleSelect:
                            case ComponentType.SelectMenu:
                            case ComponentType.UserSelect:
                                {
                                    await HttpRespondWithError(Request, Interaction, "Select menu is not supported with Nova Node yet.", cancellationToken);
                                    return;
                                }
                                break;
                        }
                    }
                    break;
                case RestModal md:
                    {
                        AppData.GetEvents().TriggerEvent(new AppLogItem()
                        {
                            Title = $"Modal Interaction: {Interaction.User.Username} ran {md.Id} in #{Interaction.Channel.Name}",
                            Data = body,
                            Type = WorkspaceType.DiscordInteractionModal
                        });

                        string[] SplitId = md.Data.CustomId.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        string Id = SplitId[0];

                        Console.WriteLine("GOT MODAL: " + string.Join("|", SplitId));

                        if (AppData.ModalInteractions.TryGetValue(Id, out IDiscordAppInteraction bi))
                            InteractionData = bi;

                        if (SplitId.Length == 2)
                            outputDataType = SplitId[1];

                        Type = WorkspaceType.DiscordInteractionModal;
                    }
                    break;
                case RestAutocompleteInteraction ac:
                    {
                        await HttpRespondWithError(Request, Interaction, "Auto complete is not supported with Nova Node yet.", cancellationToken);
                        return;
                    }
                    break;
            }

            if (InteractionData == null)
            {
                await HttpRespondWithError(Request, Interaction, "This interaction has no data.", cancellationToken);
                return;
            }

            if (InteractionData.WorkspaceId == null)
            {
                await HttpRespondWithError(Request, Interaction, "This command group has no commands created.", cancellationToken);
                return;
            }

            if (!InteractionData.IsEnabled && AppData.BotOwnerId != Interaction.User.Id.ToString() && !AppData.BotDeveloperIds.Contains(Interaction.User.Id.ToString()))
            {
                await HttpRespondWithError(Request, Interaction, "This interaction has been disabled by the developer.", cancellationToken);
                return;
            }

            DiscordRuntime runtime = new DiscordRuntime(AppData, Client, Interaction, InteractionData, InteractionData.IsEphemeral, long.Parse(timestamp), currentTime);
            runtime.Type = Type;

            WorkspaceBlock? commandBlock = AppData.GetCache().GetCacheCommand(InteractionData.WorkspaceId.Value);
            if (commandBlock == null)
            {
                await HttpRespondWithError(Request, Interaction, "This interaction has no data.", cancellationToken);
                return;
            }


            WorkspaceBlock? firstOption = null;
            if (commandBlock.inputs.TryGetValue("command_options", out WorkspaceBlockConnection? actBlock) && actBlock.block != null)
                firstOption = actBlock.block;


            if (!string.IsNullOrEmpty(InteractionData.OpenModal))
            {
                if (!AppData.ModalInteractions.TryGetValue(InteractionData.OpenModal, out IDiscordAppInteraction modalInteraction))
                {
                    await HttpRespondWithError(Request, Interaction, $"Invalid interaction modal name {InteractionData.OpenModal}.", cancellationToken);
                    return;
                }

                WorkspaceBlock? modalBlock = AppData.GetCache().GetCacheCommand(modalInteraction.WorkspaceId.Value);


                if (!string.IsNullOrEmpty(InteractionData.ModalDataType))
                {
                    if (modalBlock.inputs.TryGetValue("output_data", out WorkspaceBlockConnection? modalOutputBlock) && modalOutputBlock.block != null && modalOutputBlock.block.enabled)
                    {
                        Console.WriteLine("Modal Data: " + InteractionData.ModalDataType);

                        await runtime.RunInitialInputs(commandBlock, outputDataType);

                        RuntimeError? error = null;

                        if (firstOption != null)
                            error = await runtime.RunInitialOptions(firstOption);

                        if (error != null)
                        {
                            await runtime.RunError(error, Request, cancellationToken);
                            return;
                        }

                        Console.WriteLine($"Copy: {InteractionData.ModalDataType} with {modalOutputBlock.block.type}");
                        runtime.CopyModalData(new WorkspaceBlock { type = InteractionData.ModalDataType }, modalOutputBlock.block);
                        WorkspaceBlock? firstModalOption = null;
                        if (modalBlock.inputs.TryGetValue("command_options", out WorkspaceBlockConnection? optBlock) && optBlock.block != null)
                            firstModalOption = optBlock.block;

                        if (error == null && firstModalOption != null)
                            error = await runtime.RunInitialOptions(firstModalOption);

                        if (error != null)
                        {
                            await runtime.RunError(error, Request, cancellationToken);
                            return;
                        }

                        //Console.WriteLine("--- Modal ---");
                        //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(runtime.Data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
                        //{
                        //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                        //    ContractResolver = new Discord.Net.Converters.DiscordContractResolver()
                        //}));
                        //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(runtime.Variables.Keys, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
                        //{
                        //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                        //    ContractResolver = new Discord.Net.Converters.DiscordContractResolver()
                        //}));
                        Console.WriteLine("Runtime: " + runtime.ModalDataType);

                    }
                }

                Tuple<Modal?, RuntimeError?> modal = await ModalUtils.ParseDynamicModal(runtime, InteractionData.OpenModal, modalBlock);

                if (modal.Item2 != null)
                {
                    await HttpRespondWithError(Request, Interaction, modal.Item2.ErrorMessage, cancellationToken);
                    return;
                }

                if (modal.Item1 == null)
                {
                    await HttpRespondWithError(Request, Interaction, "Invalid interaction modal data.", cancellationToken);
                    return;
                }

                if (string.IsNullOrEmpty(modal.Item1.Title))
                {
                    await HttpRespondWithError(Request, Interaction, $"Modal {InteractionData.OpenModal} has an empty title.", cancellationToken);
                    return;
                }

                if (modal.Item1.Component.Components == null)
                {
                    await HttpRespondWithError(Request, Interaction, $"Modal {InteractionData.OpenModal} has invalid fields.", cancellationToken);
                    return;
                }


                await HttpResponseWithModal(Request, Interaction, modal.Item1, cancellationToken);

                return;
            }

            WorkspaceBlock? firstAction = null;
            if (commandBlock.inputs.TryGetValue("command_actions", out actBlock) && actBlock.block != null)
                firstAction = actBlock.block;
            if (firstAction == null)
            {
                await HttpRespondWithError(Request, Interaction, "This interaction has no actions.", cancellationToken);
                return;
            }

            Request.HttpContext.Response.Headers.Append("Content-Type", "application/json");
            await Request.HttpContext.Response.WriteAsync(Interaction.Defer(InteractionData.IsEphemeral), cancellationToken);
            await Request.HttpContext.Response.CompleteAsync();

            await runtime.RunInitialInputs(commandBlock, outputDataType);

            //Console.WriteLine("--- Run Command ---");
            //Console.WriteLine("Output: " + outputDataType);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(runtime.Data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            //    ContractResolver = new Discord.Net.Converters.DiscordContractResolver()
            //}));
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(runtime.Variables.Keys, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            //    ContractResolver = new Discord.Net.Converters.DiscordContractResolver()
            //}));

            _ = runtime.RunCommand(commandBlock, firstAction, firstOption);


        }
    }

}
