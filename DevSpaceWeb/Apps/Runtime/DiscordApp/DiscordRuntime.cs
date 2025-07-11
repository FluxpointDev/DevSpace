using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.Webhook;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public class DiscordRuntime : IRuntime
{
    public DiscordRuntime(AppData app, DiscordRestClient client, RestInteraction interaction, IDiscordAppInteraction data, bool isEphemeral, long unixResponseTime, DateTime currentTime)
    {
        UnixResponseTime = unixResponseTime;
        CurrentResponseTime = currentTime;
        AppData = app;
        if (isEphemeral)
        {
            Options = new RuntimeOptions { IsEphemeral = true };
        }
        Interaction = interaction;
        InteractionData = data;
        Client = client;

        Data.ChannelCurrent = interaction.Channel as RestChannel;
        Cache.Channels.Add(interaction.Channel.Id, interaction.Channel as RestChannel);

        Data.UserCurrent = interaction.User;
        Cache.Users.Add(interaction.User.Id, interaction.User);

        Data.UserApp = Client.CurrentUser;

        if (interaction.Channel is RestTextChannel gc && gc.CategoryId.HasValue)
            Data.CategoryCurrent = Tuple.Create<ulong, RestCategoryChannel?>(gc.CategoryId.Value, null);

        if (interaction.Guild != null)
        {
            Data.ServerCurrent = Tuple.Create<ulong, RestGuild?>(interaction.GuildId!.Value, interaction.Guild);
            Data.MemberApp = Tuple.Create<ulong, RestGuildUser?>(interaction.ApplicationId, null);

            Data.MemberCurrent = (interaction.User as RestGuildUser);
            Cache.AddMember(Data.MemberCurrent!);
        }
        else if (interaction.GuildId.HasValue)
        {
            Data.ServerCurrent = Tuple.Create<ulong, RestGuild?>(interaction.GuildId!.Value, null);
            Data.MemberApp = Tuple.Create<ulong, RestGuildUser?>(interaction.ApplicationId, null);

            Data.MemberCurrent = (interaction.User as RestGuildUser);
            Cache.AddMember(Data.MemberCurrent!);
        }
    }

    public AppData AppData { get; private set; }

    public RestFollowupMessage? InteractionResponse = null;

    public AllowedMentions? AllowedMentions = new AllowedMentions { AllowedTypes = AllowedMentionTypes.None };

    public string ModalDataType { get; set; }

    public RuntimeOptions Options = null;

    public DiscordCache Cache = new DiscordCache();

    public DiscordData Data = new DiscordData();

    public RestInteraction Interaction { get; private set; }

    public IDiscordAppInteraction InteractionData { get; private set; }
    public DiscordRestClient Client { get; private set; }

    public WorkspaceType Type { get; set; }

    public RuntimeError GetAppPermissionError(GuildPermission perm)
    {
        return new RuntimeError(RuntimeErrorType.Runtime, $"This app **{Client.CurrentUser.Username}** does not have server permission for `{FriendlyPermissionName(perm)}`\n" +
            $"Please check the roles and channel permissions to give access.");
    }

    public string FriendlyPermissionName(GuildPermission perm)
    {
        return InternalPermissionName(perm.ToString());
    }
    public string FriendlyPermissionName(ChannelPermission perm)
    {
        return InternalPermissionName(perm.ToString());
    }

    private string InternalPermissionName(string type)
    {
        switch (type)
        {
            case "CreateInstantInvite":
                return "Create Invite";
            case "KickMembers":
                return "Kick Members";
            case "BanMembers":
                return "Ban Members";
            case "Administrator":
                return "Administrator";
            case "ManageChannels":
                return "Manage Channels";
            case "ManageGuild":
                return "Manage Server";
            case "ViewGuildInsights":
                return "View Server Insights";
            case "AddReactions":
                return "Add Reactions";
            case "ViewAuditLog":
                return "View Audit Log";
            case "ViewChannel":
                return "View Channel";
            case "SendMessages":
                return "Send Messages";
            case "SendTTSMessages":
                return "Send TTS Messages";
            case "ManageMessages":
                return "Manage Messages";
            case "EmbedLinks":
                return "Embed Links";
            case "AttachFiles":
                return "Attach Files";
            case "ReadMessageHistory":
                return "Read Message History";
            case "MentionEveryone":
                return "Mention Everyone";
            case "UseExternalEmojis":
                return "Use External Emojis";
            case "Connect":
                return "Voice Connect";
            case "Speak":
                return "Voice Speak";
            case "MuteMembers":
                return "Voice Mute Members";
            case "DeafenMembers":
                return "Voice Deafen Members";
            case "MoveMembers":
                return "Voice Move Members";
            case "UseVAD":
                return "Use Voice Activity";
            case "PrioritySpeaker":
                return "Voice Priority Speaker";
            case "Stream":
                return "Voice Stream/Video";
            case "ChangeNickname":
                return "Change Nickname";
            case "ManageNicknames":
                return "Manage Nicknames";
            case "ManageRoles":
                return "Manage Roles";
            case "ManageWebhooks":
                return "Manage Webhooks";
            case "ManageEmojisAndStickers":
                return "Manage Emojis & Stickers";
            case "UseApplicationCommands":
                return "Use Application Commands";
            case "RequestToSpeak":
                return "Request to Speak";
            case "ManageEvents":
                return "Manage Events";
            case "ManageThreads":
                return "Manage Threads";
            case "CreatePublicThreads":
                return "Create Public Threads";
            case "CreatePrivateThreads":
                return "Create Private Threads";
            case "UseExternalStickers":
                return "Use External Stickers";
            case "SendMessagesInThreads":
                return "Send Messages in Threads";
            case "StartEmbeddedActivities":
                return "Start Activity";
            case "ModerateMembers":
                return "Moderate Members";
            case "ViewMonetizationAnalytics":
                return "View Creator Monetization";
            case "UseSoundboard":
                return "Use Soundboard";
            case "CreateGuildExpressions":
                return "Create Expressions";
            case "CreateEvents":
                return "Create Events";
            case "UseExternalSounds":
                return "Use External Sounds";
            case "SendVoiceMessages":
                return "Send Voice Messages";
            case "SetVoiceChannelStatus":
                return "Set Voice Channel Status";
        }

        return "ERROR!!!";
    }

    public bool IsEphemeral()
    {
        if (Options != null)
            return Options.IsEphemeral;

        return false;
    }

    public async Task RunInitialInputs(WorkspaceBlock commandblock, string outputDataType)
    {
        switch (Type)
        {
            case WorkspaceType.DiscordSlashCommand:
                {
                    if (commandblock.inputs.TryGetValue("command_inputs", out WorkspaceBlockConnection? cmdI) && cmdI.block != null)
                        await DiscordInputs.ParseNextBlock(this, cmdI.block, InteractionData.ModalDataType);
                }
                break;
            case WorkspaceType.DiscordUserCommand:
                {
                    if (commandblock.inputs.TryGetValue("output_user", out WorkspaceBlockConnection? outputBlock) && outputBlock.block != null && outputBlock.block.enabled)
                    {
                        if (Interaction.GuildId.HasValue && !outputBlock.block.type.StartsWith("data_user"))
                        {
                            SetMemberData(outputBlock.block, (Interaction as RestUserCommand).Data.Member as RestGuildUser);

                            if (!string.IsNullOrEmpty(outputDataType) && outputDataType == outputBlock.block.type)
                            {
                                this.ModalDataType = $"mb{((Interaction as RestUserCommand).Data.Member as RestGuildUser).GuildId}-{(Interaction as RestUserCommand).Data.Member.Id}";
                            }
                        }
                        else if (outputBlock.block.type.StartsWith("data_user"))
                        {
                            SetUserData(outputBlock.block, (Interaction as RestUserCommand).Data.Member as RestUser);

                            if (!string.IsNullOrEmpty(outputDataType) && outputDataType == outputBlock.block.type)
                            {
                                this.ModalDataType = $"ur{(Interaction as RestUserCommand).Data.Member.Id}";
                            }
                        }
                    }
                }
                break;
            case WorkspaceType.DiscordMessageCommand:
                {
                    if (commandblock.inputs.TryGetValue("output_message", out WorkspaceBlockConnection? outputBlock) && outputBlock.block != null && outputBlock.block.enabled)
                    {
                        SetMessageData(outputBlock.block, (Interaction as RestMessageCommand).Data.Message as RestMessage);

                        if (!string.IsNullOrEmpty(outputDataType) && outputDataType == outputBlock.block.type)
                            this.ModalDataType = $"ms{(Interaction as RestMessageCommand).Data.Message.Channel.Id}-{(Interaction as RestMessageCommand).Data.Message.Id}";
                    }
                }
                break;
            case WorkspaceType.DiscordInteractionButton:
                {
                    Data.MessageCurrent = Tuple.Create<DiscordCachableMessage, RestMessage>(new DiscordCachableMessage((Interaction as RestMessageComponent).Message.Channel.Id, (Interaction as RestMessageComponent).Message.Id), (Interaction as RestMessageComponent).Message);

                    if (commandblock.inputs.TryGetValue("output_data", out WorkspaceBlockConnection? outputBlock) && outputBlock.block != null && outputBlock.block.enabled)
                    {
                        Console.WriteLine("Parse Button!! " + outputDataType);
                        if (!string.IsNullOrEmpty(outputDataType))
                            ParseDataType(outputBlock.block, outputDataType);

                    }
                }
                break;
            case WorkspaceType.DiscordInteractionModal:
                {
                    //if ((Interaction as RestModal).Message != null)
                    //    Data.MessageCurrent = Tuple.Create<DiscordCachableMessage, RestMessage>(new DiscordCachableMessage((Interaction as RestModal).Message.Channel.Id, (Interaction as RestModal).Message.Id), (Interaction as RestModal).Message);


                    if (commandblock.inputs.TryGetValue("output_data", out WorkspaceBlockConnection? outputBlock) && outputBlock.block != null && outputBlock.block.enabled)
                    {
                        Console.WriteLine("Parse Modal!! " + outputDataType);
                        if (!string.IsNullOrEmpty(outputDataType))
                            ParseDataType(outputBlock.block, outputDataType);
                    }

                    if (commandblock.inputs.TryGetValue("modal_fields", out WorkspaceBlockConnection? actBlock) && actBlock.block != null)
                    {
                        await DiscordFields.Parse(this, actBlock.block);
                    }
                }
                break;


        }
    }

    public async Task<RuntimeError?> RunInitialOptions(WorkspaceBlock optionBlock)
    {
        RuntimeError? error = await DiscordOptions.Parse(this, optionBlock);
        if (error != null)
            return error;


        if (Options != null)
        {
            if (Options.RequireAppDeveloper)
            {
                bool HasOwner = false;
                if (!string.IsNullOrEmpty(AppData.BotOwnerId) && AppData.BotOwnerId == Interaction.User.Id.ToString())
                    HasOwner = true;

                if (!HasOwner)
                    HasOwner = AppData.BotDeveloperIds.Contains(Interaction.User.Id.ToString());

                if (!HasOwner)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires the app owner.");

            }
            if (Options.RequireAppOwner)
            {
                if (string.IsNullOrEmpty(AppData.BotOwnerId) || AppData.BotOwnerId != Interaction.User.Id.ToString())
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires the app owner.");

            }
            if (Options.RequireAppPremium)
            {
                if (Interaction.Entitlements.Count == 0)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires an app premium subscription using Discord.");
            }

            if (Options.RequireNsfw)
            {
                if (!Interaction.GuildId.HasValue)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server with an nsfw channel.");

                bool IsNsfw = false;

                if (Interaction.Channel is RestTextChannel rg)
                    IsNsfw = rg.IsNsfw;
                else if (Interaction.Channel is RestForumChannel fg)
                    IsNsfw = fg.IsNsfw;

                if (!IsNsfw)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires an nsfw channel.");
            }

            switch (Interaction.Channel.GetChannelType())
            {
                case ChannelType.DM:
                    {
                        if (Options.RequireServer)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server.");

                        if (Options.RequireGroupChannel)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a group channel.");
                    }
                    break;
                case ChannelType.Group:
                    {
                        if (Options.RequireServer)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server.");

                        if (Options.RequirePrivateChannel)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a private/dm channel.");
                    }
                    break;
                default:
                    {
                        if (Options.RequirePrivateChannel)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a private/dm channel.");

                        if (Options.RequireGroupChannel)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a group channel.");
                    }
                    break;
            }
            if (Options.RequireServerOwner)
            {
                if (!Interaction.GuildId.HasValue)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server.");

                if (Interaction.Guild == null)
                    await Interaction.GetGuildAsync(new RequestOptions());

                if (Interaction.Guild.OwnerId != Interaction.User.Id)
                    return new RuntimeError(RuntimeErrorType.Runtime, "You need to be the server owner to use this command.");
            }
            if (Options.RequireServerPermissions.Count != 0)
            {
                if (!Interaction.GuildId.HasValue)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server.");


                foreach (Tuple<GuildPermissions, WorkspaceBlock> i in Options.RequireServerPermissions)
                {
                    ulong? UserId = await GetMemberIdFromBlock(i.Item2);
                    if (!UserId.HasValue)
                        return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require server permission has an invalid user.");

                    bool HasPermissions = false;

                    if (UserId == Interaction.User.Id)
                    {
                        GuildPermission? CheckPermission = null;
                        foreach (GuildPermission p in i.Item1.ToList())
                        {
                            CheckPermission = p;
                            HasPermissions = DiscordPermissions.HasPermission(p, (Interaction.User as RestGuildUser).InteractionGuildPermissions.Value);
                            if (!HasPermissions)
                                break;
                        }



                        if (!HasPermissions)
                        {
                            if (!CheckPermission.HasValue)
                                return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require server permission failed to validate permissions.");
                            return new RuntimeError(RuntimeErrorType.Runtime, $"You do not have server permission for `{FriendlyPermissionName(CheckPermission!.Value)}`\n" +
                                    $"Please check the roles permissions to give access.");
                        }
                    }
                    else
                    {
                        RestGuildUser? AppMember = await GetMemberAsync(Interaction.GuildId.Value, UserId.Value);

                        if (AppMember == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require server permission has an invalid input user.");

                        GuildPermission? CheckPermission = null;

                        foreach (GuildPermission p in i.Item1.ToList())
                        {
                            CheckPermission = p;
                            HasPermissions = DiscordPermissions.HasPermission(p, AppMember.InteractionGuildPermissions.HasValue ? AppMember.InteractionGuildPermissions.Value : AppMember.GuildPermissions);
                            if (!HasPermissions)
                                break;
                        }

                        if (!HasPermissions)
                        {
                            if (!CheckPermission.HasValue)
                                return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require server permission failed to validate permissions.");

                            if (UserId.Value == Client.CurrentUser.Id)
                            {
                                return new RuntimeError(RuntimeErrorType.Runtime, $"This app **{AppMember.Username}** does not have server permission for `{FriendlyPermissionName(CheckPermission!.Value)}`\n" +
                                    $"Please check the roles permissions to give access.");
                            }
                            else
                            {
                                return new RuntimeError(RuntimeErrorType.Runtime, $"This user **{AppMember.Username}** does not have server permission `{FriendlyPermissionName(CheckPermission!.Value)}`\n" +
                                $"Please check the roles permissions to give access.");
                            }
                        }

                    }
                }
            }

            if (Options.RequireChannelPermissions.Count != 0)
            {
                if (!Interaction.GuildId.HasValue)
                    return new RuntimeError(RuntimeErrorType.Runtime, "This command requires a Discord server for permission checks.");

                foreach (Tuple<ChannelPermissions, WorkspaceBlock> i in Options.RequireChannelPermissions)
                {
                    ulong? ChanId = await GetChannelIdFromBlock(i.Item2);
                    if (!ChanId.HasValue)
                        return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission has an invalid channel.");

                    ulong? UserId = await GetMemberIdFromBlock(i.Item2.next.block);
                    if (!UserId.HasValue)
                        return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission has an invalid user.");

                    RestChannel? Channel = await GetChannelAsync(ChanId.Value);
                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission has an invalid channel.");

                    bool HasPermissions = false;

                    if (UserId == Interaction.User.Id)
                    {
                        ChannelPermission? CheckPermission = null;
                        foreach (ChannelPermission p in i.Item1.ToList())
                        {

                            CheckPermission = p;
                            HasPermissions = DiscordPermissions.HasPermission(p, Interaction.ChannelPermissions);
                            if (!HasPermissions)
                                break;
                        }

                        if (!HasPermissions)
                        {
                            if (!CheckPermission.HasValue)
                                return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission failed to validate permissions.");

                            return new RuntimeError(RuntimeErrorType.Runtime, $"You do not have channel permission `{FriendlyPermissionName(CheckPermission!.Value)}` for <#{ChanId}>\n" +
                                    $"Please check the channel permissions to give access.");
                        }

                    }
                    else
                    {
                        RestGuildUser? AppMember = await GetMemberAsync(Interaction.GuildId.Value, UserId.Value);

                        if (AppMember == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission has an invalid input user.");

                        ChannelPermission? CheckPermission = null;

                        foreach (ChannelPermission p in i.Item1.ToList())
                        {
                            CheckPermission = p;
                            HasPermissions = DiscordPermissions.HasPermission(p, AppMember.GetPermissions(Channel as IGuildChannel));
                            if (!HasPermissions)
                                break;
                        }

                        if (!HasPermissions)
                        {
                            if (!CheckPermission.HasValue)
                                return new RuntimeError(RuntimeErrorType.Runtime, "This command has an issue, require channel permission failed to validate permissions.");

                            if (UserId.Value == Client.CurrentUser.Id)
                                return new RuntimeError(RuntimeErrorType.Runtime, $"This app **{AppMember.Username}** does not have channel permission `{FriendlyPermissionName(CheckPermission!.Value)}` for <#{ChanId}>\n" +
                                            $"Please check the channel permissions to give access.");
                            else
                                return new RuntimeError(RuntimeErrorType.Runtime, $"This user **{AppMember.Username}** does not have channel permission `{FriendlyPermissionName(CheckPermission!.Value)}` for <#{ChanId}>\n" +
                                        $"Please check the channel permissions to give access.");
                        }
                    }
                }
            }
        }

        return null;
    }

    public async Task RunCommand(WorkspaceBlock commandblock, WorkspaceBlock firstActionBlock, WorkspaceBlock? firstOptionBlock)
    {
        RuntimeError? error = null;
        try
        {
            if (firstOptionBlock != null)
                error = await RunInitialOptions(firstOptionBlock);

            if (error == null)
                error = await RunCommandInternal(commandblock, firstActionBlock);
        }
        catch (RuntimeError re)
        {
            error = re;
        }
        catch (HttpRequestException he)
        {
            error = new RuntimeError(RuntimeErrorType.Request, he.Message);
        }
        catch (HttpException ex)
        {
            error = new RuntimeError(RuntimeErrorType.Discord, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            //Program.Sentry.SendEvent(ex, "Discord Runtime", $"Runtime failed for bot {Client.CurrentUser.Username}", new Sentry.Protocol.User
            //{
            //    Id = Interaction.User.Id.ToString(),
            //    Username = Interaction.User.Username
            //}, Sentry.Protocol.SentryLevel.Fatal);

            error = new RuntimeError(RuntimeErrorType.Server, "Server error occured!");
        }

        if (error != null && error.Type != RuntimeErrorType.StopExecution)
        {
            _ = RunError(error);
        }
        else
        {
            foreach (KeyValuePair<ulong, DiscordWebhookClient> i in Cache.WebhookClients)
            {
                i.Value.Dispose();
            }

            if (InteractionResponse == null)
            {
                _ = Interaction.FollowupAsync("Command done");
            }
        }
    }

    public async Task RunError(RuntimeError error, HttpRequest? request = null, CancellationToken? cancellationToken = null)
    {
        foreach (KeyValuePair<ulong, DiscordWebhookClient> i in Cache.WebhookClients)
        {
            i.Value.Dispose();
        }
        string ErrorType = "Interaction";
        switch (Interaction.Type)
        {
            case InteractionType.ApplicationCommand:
            case InteractionType.ApplicationCommandAutocomplete:
                ErrorType = "Command";
                break;
            case InteractionType.ModalSubmit:
                ErrorType = "Modal";
                break;
        }
        switch (error.Type)
        {
            case RuntimeErrorType.Discord:
                ErrorType = "Discord";
                break;
            case RuntimeErrorType.Request:
                ErrorType = "Request";
                break;
            case RuntimeErrorType.Server:
                ErrorType = "Server";
                break;
        }

        string Icon = ":x:";
        if (error.Type != RuntimeErrorType.Runtime)
            Icon = ":warning:";

        if (InteractionResponse != null)
            if (InteractionResponse.Content.Length > 3000)
            {
                _ = Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Content = $"{Icon} **{ErrorType} Error**\n" +
                    error.ErrorMessage;
                    x.AllowedMentions = new AllowedMentions();
                });
            }
            else
            {
                _ = Interaction.ModifyOriginalResponseAsync(x =>
                {
                    x.Content = InteractionResponse.Content + "\n\n" +
                    $"{Icon} **{ErrorType} Error**\n" +
                    error.ErrorMessage;
                    x.AllowedMentions = new AllowedMentions();
                });
            }

        else
        {
            if (request == null)
                _ = Interaction.FollowupAsync($"{Icon} **{ErrorType} Error**\n" + error.ErrorMessage, ephemeral: IsEphemeral(), allowedMentions: new AllowedMentions());
            else
            {
                request.HttpContext.Response.Headers.Append("Content-Type", "application/json");
                await request.HttpContext.Response.WriteAsync(Interaction.Respond($"{Icon} **{ErrorType} Error**\n" + error.ErrorMessage, ephemeral: true), cancellationToken.Value);
                await request.HttpContext.Response.CompleteAsync();
            }
        }
    }

    public async Task<RestGuild?> GetGuildAsync(ulong id)
    {
        if (Cache.Servers.TryGetValue(id, out RestGuild? guild))
            return guild;

        guild = await Client.GetGuildAsync(id);
        if (guild != null)
            Cache.AddServer(guild);

        return guild;
    }

    public async Task<GuildEmote?> GetGuildEmoji(ulong serverId, ulong emojiId)
    {
        if (Cache.Emojis.TryGetValue(emojiId, out Tuple<RestGuild, GuildEmote>? emoji))
            return emoji.Item2;

        RestGuild? server = await GetGuildAsync(serverId);
        if (server == null)
            return null;

        GuildEmote? getemoji = await server.GetEmoteAsync(emojiId);
        if (getemoji != null)
            Cache.AddEmoji(server, getemoji);

        return getemoji;
    }


    public async Task<RestRole?> GetRoleAsync(ulong serverId, ulong roleId)
    {
        if (Cache.Roles.TryGetValue(roleId, out RestRole? role))
            return role;

        RestGuild? server = await GetGuildAsync(serverId);
        if (server == null)
            return null;

        role = server.GetRole(roleId);

        if (role != null)
            Cache.AddRole(role);

        return role;
    }

    public async Task<RestGuildUser?> GetMemberAsync(ulong serverId, ulong userId)
    {
        if (Cache.Members.TryGetValue($"{serverId}-{userId}", out RestGuildUser? member))
            return member;

        member = await Client.GetGuildUserAsync(serverId, userId);

        if (member != null)
            Cache.AddMember(member);

        return member;
    }

    public async Task<RestUser?> GetUserAsync(ulong id)
    {
        if (id == Client.CurrentUser.Id)
            return Client.CurrentUser;

        if (Cache.Users.TryGetValue(id, out RestUser? user))
            return user;

        user = await Client.GetUserAsync(id);

        if (user != null)
            Cache.AddUser(user);

        return user;
    }

    public async Task<RestChannel?> GetChannelAsync(ulong id)
    {
        if (Cache.Channels.TryGetValue(id, out RestChannel? chan))
            return chan;

        chan = await Client.GetChannelAsync(id);

        if (chan != null)
            Cache.AddChannel(chan);

        return chan;
    }

    public async Task<RestDMChannel?> GetDMChannelAsync(ulong id)
    {
        if (Cache.Channels.TryGetValue(id, out RestChannel? chan))
            return chan as RestDMChannel;

        chan = await Client.GetDMChannelAsync(id) as RestDMChannel;

        if (chan != null)
            Cache.AddChannel(chan);

        return chan as RestDMChannel;
    }

    public async Task<RestGroupChannel?> GetGroupChannelAsync(ulong id)
    {
        if (Cache.Channels.TryGetValue(id, out RestChannel? chan))
            return chan as RestGroupChannel;

        chan = await Client.GetGroupChannelAsync(id) as RestGroupChannel;

        if (chan != null)
            Cache.AddChannel(chan);

        return chan as RestGroupChannel;
    }

    public async Task<Tuple<RestWebhook?, string>?> GetWebhookAsync(ulong id, string url)
    {
        if (Cache.Webhooks.TryGetValue(id, out Tuple<RestWebhook?, string>? wh))
            return wh;

        RestWebhook getwh = await Client.GetWebhookAsync(id);

        if (getwh != null)
            return Tuple.Create<RestWebhook?, string>(getwh, "https://discord.com/api/webhooks/" + getwh.Id + "/" + getwh.Token);

        return null;
    }

    public async Task<RestCategoryChannel?> GetCategoryAsync(ulong id)
    {
        if (Cache.Category.TryGetValue(id, out RestCategoryChannel? cat))
            return cat;

        cat = await Client.GetChannelAsync(id) as RestCategoryChannel;

        if (cat != null)
            Cache.AddCategory(cat);

        return cat;
    }

    public async Task<RestMessage?> GetMessageAsync(ulong chan, ulong msg)
    {
        if (Cache.Messages.TryGetValue(msg, out RestMessage message))
            return message;

        IRestMessageChannel? GetChan = await GetChannelAsync(chan) as IRestMessageChannel;

        if (GetChan == null)
            return null;

        message = await GetChan.GetMessageAsync(msg);

        if (message != null)
            Cache.AddMessage(message);

        return message;
    }

    public void ParseDataType(WorkspaceBlock outputBlock, string type)
    {
        string DataType = type.Substring(0, 2);
        string DataValue = type.Substring(2);
        Console.WriteLine($"Parse Data Type: {DataType} - {DataValue} - {outputBlock.type}");
        switch (DataType)
        {
            // Server
            case "sv":
                {
                    SetServerData(outputBlock, ulong.Parse(DataValue));
                }
                break;
            // Role
            case "rl":
                {
                    string[] SplitData = DataValue.Split('-');

                    SetRoleData(outputBlock, ulong.Parse(SplitData[0]), ulong.Parse(SplitData[1]));
                }
                break;
            // Channel
            case "ch":
                {
                    SetChannelData(outputBlock, ulong.Parse(DataValue));
                }
                break;
            // Category
            case "ct":
                {
                    SetCategoryData(outputBlock, ulong.Parse(DataValue));
                }
                break;
            // Message
            case "ms":
                {
                    string[] SplitData = DataValue.Split('-');

                    SetMessageData(outputBlock, ulong.Parse(SplitData[0]), ulong.Parse(SplitData[1]));
                }
                break;
            // Emoji
            case "em":
                {
                    string[] SplitData = DataValue.Split('-');

                    if (Emote.TryParse(SplitData[1], out Emote emote))
                        SetEmojiData(outputBlock, ulong.Parse(SplitData[0]), emote);

                    if (Emoji.TryParse(SplitData[1], out Emoji emo))
                        SetEmojiData(outputBlock, ulong.Parse(SplitData[0]), emo);


                }
                break;
            // Member
            case "mb":
                {
                    string[] SplitData = DataValue.Split('-');

                    SetMemberData(outputBlock, ulong.Parse(SplitData[0]), ulong.Parse(SplitData[1]));
                }
                break;
            // User
            case "ur":
                {
                    SetUserData(outputBlock, ulong.Parse(DataValue));
                }
                break;
        }
    }


    public async Task<RuntimeError?> RunCommandInternal(WorkspaceBlock commandblock, WorkspaceBlock firstActionBlock)
    {
        return await RunAction(firstActionBlock);
    }

    public async Task<RuntimeError?> RunAction(WorkspaceBlock block)
    {
        if (block.enabled)
        {
            switch (block.type)
            {
                case "action_stop_execution":

                    if (block.inputs.TryGetValue("obj_message", out WorkspaceBlockConnection? MessageData) && MessageData.block != null)
                    {
                        RuntimeError? Error = await new SendMessageBlock()
                        {
                            Block = block,
                            Client = Client,
                            Runtime = this
                        }.RunAsync();
                        if (Error != null)
                            return Error;
                    }

                    return new RuntimeError(RuntimeErrorType.StopExecution, null)
                    {

                    };
                case "controls_if":
                    bool IsActionAllowed = false;

                    foreach (KeyValuePair<string, WorkspaceBlockConnection> i in block.inputs)
                    {
                        if (i.Value.block == null)
                            continue;

                        if (i.Key.StartsWith("IF"))
                        {
                            RuntimeError? result = null;
                            try
                            {
                                IsActionAllowed = await DiscordLogic.Parse(this, i.Value.block);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                result = new RuntimeError(RuntimeErrorType.Server, "Failed to run logic condition due to server issue.");
                            }
                            if (result != null)
                                return result;
                        }
                        else
                        {
                            if (i.Key == "ELSE" || IsActionAllowed)
                            {
                                RuntimeError? result = await RunAction(i.Value.block);

                                if (result != null)
                                    return result;
                                break;
                            }
                        }
                    }

                    break;
                case "block_try_catch":
                    bool IsDisabled = false;
                    if (block.inputs.TryGetValue("is_disabled", out WorkspaceBlockConnection? dsBlock) && dsBlock.block != null)
                        IsDisabled = (await GetBoolFromBlock(dsBlock.block)).GetValueOrDefault(false);

                    if (block.inputs.TryGetValue("command_actions", out WorkspaceBlockConnection? cmdBlock) && cmdBlock.block != null)
                    {
                        if (!IsDisabled)
                        {
                            try
                            {
                                RuntimeError? res = await RunAction(cmdBlock.block);
                                if (res != null && (res.Type == RuntimeErrorType.Server || res.Type == RuntimeErrorType.StopExecution))
                                    return res;
                            }
                            catch (RuntimeError re)
                            {
                                if (re.Type == RuntimeErrorType.Server || re.Type == RuntimeErrorType.StopExecution)
                                    return re;
                            }
                            catch { }
                        }
                        else
                        {
                            RuntimeError? res = await RunAction(cmdBlock.block);
                            if (res != null)
                                return res;
                        }
                    }
                    break;
                default:
                    {
                        IActionBlock? BlockType = DiscordBlocks.ParseAction(this, block);
                        if (BlockType == null)
                            return new RuntimeError(RuntimeErrorType.Server, "Failed to run command action type.");

                        RuntimeError? result = await BlockType.RunAsync();
                        if (result != null)
                            return result;
                    }
                    break;
            }
        }

        if (block.next != null && block.next.block != null)
        {
            RuntimeError? nextRes = await RunAction(block.next.block);
            if (nextRes != null)
                return nextRes;
        }

        return null;
    }

    public override async Task<string> GetStringFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "data_string_interaction_token":
                return Interaction.Token;
        }
        if (block.type.StartsWith("data_selector_"))
        {
            object? data = await DiscordSelectors.Parse(this, block);
            if (data != null)
                return data.ToString();
        }
        return await GetBaseStringFromBlock(block);
    }

    public GuildPermissions? GetPermissionsFromBlock(WorkspaceBlock block)
    {
        if (block != null)
        {
            switch (block.type)
            {
                case "variables_get":
                    if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                        return obj as GuildPermissions?;
                    break;
                case "data_permission_manage":
                case "data_permission_mod":
                case "data_permission_server":
                case "data_permission_text":
                case "data_permission_voice":
                    {
                        GuildPermissions perms = new GuildPermissions();
                        ParsePermission(ref perms, block.fields["permission_type"].ToString());
                        return perms;
                    }
                case "data_permission_list":
                    {
                        GuildPermissions perms = new GuildPermissions();
                        if (block.inputs.Count == 0)
                            return null;
                        foreach (WorkspaceBlockConnection i in block.inputs.Values)
                        {
                            if (i.block != null && i.block.fields.ContainsKey("permission_type"))
                            {
                                ParsePermission(ref perms, i.block.fields["permission_type"].ToString());
                            }
                        }
                        return perms;
                    }
                case "data_permission_active":
                    {
                        if (Data.PermissionsActive != null)
                            return Data.PermissionsActive.Value;
                    }
                    break;
            }
        }
        return null;
    }

    private void ParsePermission(ref GuildPermissions perm, string type)
    {

        switch (type)
        {
            case "view_channels":
                perm = perm.Modify(viewChannel: true);
                break;
            case "manage_channels":
                perm = perm.Modify(manageChannels: true);
                break;
            case "manage_roles":
                perm = perm.Modify(manageRoles: true);
                break;
            case "create_expressions":
                perm = perm.Modify(createGuildExpressions: true);
                break;
            case "manage_expressions":
                perm = perm.Modify(manageEmojisAndStickers: true);
                break;
            case "view_audit_log":
                perm = perm.Modify(viewAuditLog: true);
                break;
            case "view_server_insights":
                perm = perm.Modify(viewGuildInsights: true);
                break;
            case "view_creator_monetization":
                perm = perm.Modify(viewMonetizationAnalytics: true);
                break;
            case "manage_webhooks":
                perm = perm.Modify(manageWebhooks: true);
                break;
            case "manage_server":
                perm = perm.Modify(manageGuild: true);
                break;
            case "create_invites":
                perm = perm.Modify(createInstantInvite: true);
                break;
            case "change_nickname":
                perm = perm.Modify(changeNickname: true);
                break;
            case "manage_nicknames":
                perm = perm.Modify(manageNicknames: true);
                break;
            case "kick_members":
                perm = perm.Modify(kickMembers: true);
                break;
            case "ban_members":
                perm = perm.Modify(banMembers: true);
                break;
            case "moderate_members":
                perm = perm.Modify(moderateMembers: true);
                break;
            case "send_messages":
                perm = perm.Modify(sendMessages: true);
                break;
            case "send_messages_threads":
                perm = perm.Modify(sendMessagesInThreads: true);
                break;
            case "create_public_threads":
                perm = perm.Modify(createPublicThreads: true);
                break;
            case "create_private_threads":
                perm = perm.Modify(createPrivateThreads: true);
                break;
            case "embed_links":
                perm = perm.Modify(embedLinks: true);
                break;
            case "attach_files":
                perm = perm.Modify(attachFiles: true);
                break;
            case "use_external_emojis":
                perm = perm.Modify(useExternalEmojis: true);
                break;
            case "use_external_stickers":
                perm = perm.Modify(useExternalStickers: true);
                break;
            case "mention_everyone":
                perm = perm.Modify(mentionEveryone: true);
                break;
            case "manage_messages":
                perm = perm.Modify(manageMessages: true);
                break;
            case "manage_threads":
                perm = perm.Modify(manageThreads: true);
                break;
            case "read_message_history":
                perm = perm.Modify(readMessageHistory: true);
                break;
            case "send_tts_messages":
                perm = perm.Modify(sendTTSMessages: true);
                break;
            case "use_application_commands":
                perm = perm.Modify(useApplicationCommands: true);
                break;
            case "send_voice_messages":
                perm = perm.Modify(sendVoiceMessages: true);
                break;
            case "voice_connect":
                perm = perm.Modify(connect: true);
                break;
            case "voice_speak":
                perm = perm.Modify(speak: true);
                break;
            case "voice_video":
                perm = perm.Modify(stream: true);
                break;
            case "use_activities":
                perm = perm.Modify(startEmbeddedActivities: true);
                break;
            case "use_soundboard":
                perm = perm.Modify(useSoundboard: true);
                break;
            case "use_external_sounds":
                perm = perm.Modify(useExternalSounds: true);
                break;
            case "use_voice_activity":
                perm = perm.Modify(useVoiceActivation: true);
                break;
            case "voice_priority_speaker":
                perm = perm.Modify(prioritySpeaker: true);
                break;
            case "voice_mute_members":
                perm = perm.Modify(muteMembers: true);
                break;
            case "voice_deafen_members":
                perm = perm.Modify(deafenMembers: true);
                break;
            case "voice_move_members":
                perm = perm.Modify(moveMembers: true);
                break;
            case "voice_set_status":
                perm = perm.Modify(setVoiceChannelStatus: true);
                break;
            case "voice_request_to_speak":
                perm = perm.Modify(requestToSpeak: true);
                break;
            case "create_events":
                perm = perm.Modify(createEvents: true);
                break;
            case "manage_events":
                perm = perm.Modify(manageEvents: true);
                break;
            case "administrator":
                perm = perm.Modify(administrator: true);
                break;
            default:
                throw new RuntimeError(RuntimeErrorType.Server, "Failed to parse permission, invalid permission type.");
        }
    }

    public async Task<RestChannel?> GetChannelFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableChannel cache)
                    {
                        RestChannel? chan = await GetChannelAsync(cache.ChannelId);
                        if (chan != null)
                            SetVariable(block.GetVariableId(), chan);

                        return chan;
                    }

                    return obj as RestChannel;
                }
                break;
            case "data_channel_current":
                {
                    return Data.ChannelCurrent;
                }
            case "data_channel_active":
                {
                    if (Data.ChannelActive == null)
                        return null;

                    if (Data.ChannelActive.Item2 != null)
                        return Data.ChannelActive.Item2;

                    RestChannel? chan = await GetChannelAsync(Data.ChannelActive.Item1);
                    if (chan != null)
                        Data.ChannelActive = Tuple.Create<ulong, RestChannel?>(chan.Id, chan);

                    return chan;

                }
            case "data_channel_get":
            case "data_channel_get_in_server":
                {
                    string ChannelId = string.Empty;
                    if (block.inputs.TryGetValue("channel_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        ChannelId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(ChannelId) && ulong.TryParse(ChannelId, out ulong id))
                    {
                        Cache.Channels.TryGetValue(id, out RestChannel? Channel);
                        if (Channel == null)
                            Channel = await GetChannelAsync(id);

                        if (Channel == null)
                            return null;

                        RestGuildChannel? Chan = Channel as RestGuildChannel;


                        if (block.type == "data_channel_get_in_server")
                        {
                            if (Channel is not RestGuildChannel)
                                return null;

                            ulong? Server = null;
                            if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                                Server = await GetServerIdFromBlock(srvBlock.block);

                            if (Server == null)
                                return null;

                            if (Chan != null && Chan is RestGuildChannel rg && rg.GuildId != Server.Value)
                                return null;
                        }

                        return Chan;


                    }
                }
                break;
            case "data_channel_private_get":
                {
                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        UserId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                    {
                        if (Cache.Channels.TryGetValue(id, out RestChannel rc))
                            return rc as RestDMChannel;

                        return await GetDMChannelAsync(id);
                    }
                }
                break;
            case "data_channel_group_get":
                {
                    string GroupId = string.Empty;
                    if (block.inputs.TryGetValue("group_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        GroupId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(GroupId) && ulong.TryParse(GroupId, out ulong id))
                    {
                        if (Cache.Channels.TryGetValue(id, out RestChannel rc))
                            return rc as RestGroupChannel;

                        return await GetGroupChannelAsync(id) as RestGroupChannel;
                    }
                }
                break;
        }

        return null;
    }

    public async Task<ulong?> GetChannelIdFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableChannel cache)
                        return cache.ChannelId;

                    if (obj is RestChannel rc)
                        return rc.Id;
                }
                break;
            case "data_channel_current":
                {
                    return Data.ChannelCurrent?.Id;
                }
            case "data_channel_active":
                {
                    if (Data.ChannelActive != null)
                        return Data.ChannelActive.Item1;
                }
                break;
            case "data_channel_get":
            case "data_channel_get_in_server":
                {
                    string ChannelId = string.Empty;
                    if (block.inputs.TryGetValue("channel_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        ChannelId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(ChannelId) && ulong.TryParse(ChannelId, out ulong id))
                    {
                        return id;
                    }
                }
                break;
            case "data_channel_private_get":
                {
                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        UserId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                    {
                        return id;
                    }
                }
                break;
            case "data_channel_group_get":
                {
                    string GroupId = string.Empty;
                    if (block.inputs.TryGetValue("group_id", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        GroupId = await GetStringFromBlock(chanBlock.block);

                    if (!string.IsNullOrEmpty(GroupId) && ulong.TryParse(GroupId, out ulong id))
                    {
                        return id;
                    }
                }
                break;
        }

        return null;
    }


    public async Task<RestCategoryChannel?> GetCategoryFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableCategory cache)
                    {
                        RestCategoryChannel? cat = await GetCategoryAsync(cache.ChannelId);

                        if (cat != null)
                            SetVariable(block.GetVariableId(), cat);

                        return cat;
                    }

                    return obj as RestCategoryChannel;
                }

                break;
            case "data_category_current":
                {
                    if (Data.CategoryCurrent == null)
                        return null;

                    if (Data.CategoryCurrent.Item2 != null)
                        return Data.CategoryCurrent.Item2;

                    RestCategoryChannel? cat = await GetChannelAsync(Data.CategoryCurrent.Item1) as RestCategoryChannel;
                    if (cat != null)
                        Data.CategoryCurrent = Tuple.Create<ulong, RestCategoryChannel?>(cat.Id, cat);

                    return cat;
                }
            case "data_category_active":
                {
                    if (Data.CategoryActive == null)
                        return null;

                    if (Data.CategoryActive.Item2 != null)
                        return Data.CategoryActive.Item2;

                    RestCategoryChannel? cat = await GetCategoryAsync(Data.CategoryActive.Item1);
                    if (cat != null)
                        Data.CategoryActive = Tuple.Create<ulong, RestCategoryChannel?>(cat.Id, cat);

                    return cat;
                }
            case "data_category_get":
            case "data_category_get_in_server":
                {
                    string CategoryId = string.Empty;
                    if (block.inputs.TryGetValue("category_id", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
                        CategoryId = await GetStringFromBlock(catBlock.block);

                    if (!string.IsNullOrEmpty(CategoryId) && ulong.TryParse(CategoryId, out ulong id))
                    {
                        Cache.Category.TryGetValue(id, out RestCategoryChannel? ccc);
                        if (ccc == null)
                            ccc = await GetCategoryAsync(id);

                        if (ccc == null)
                            return null;

                        if (block.type == "data_category_get_in_server")
                        {
                            ulong? Server = null;
                            if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                                Server = await GetServerIdFromBlock(srvBlock.block);

                            if (Server == null)
                                return null;

                            if (ccc.GetChannelType() == ChannelType.Category && ccc is RestCategoryChannel rg && rg.GuildId != Server.Value)
                                return null;
                        }

                        if (ccc.GetChannelType() == ChannelType.Category)
                        {
                            return ccc;
                        }

                    }
                }
                break;
            case "data_category_get_from_channel":
                {
                    if (block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                    {
                        RestChannel? Chan = await GetChannelFromBlock(chanBlock.block);
                        if (Chan == null)
                            return null;

                        ulong? CategoryId = null;

                        if (Chan != null)
                        {
                            if (Chan is RestTextChannel rt)
                                CategoryId = rt.CategoryId;
                            else if (Chan is RestForumChannel fc)
                                CategoryId = fc.CategoryId;
                        }

                        if (CategoryId.HasValue)
                        {
                            Cache.Category.TryGetValue(CategoryId.Value, out RestCategoryChannel? ccc);

                            if (ccc == null)
                                ccc = await GetCategoryAsync(CategoryId.Value);

                            if (ccc != null && ccc.GetChannelType() == ChannelType.Category)
                                return ccc;
                        }
                    }
                }
                break;
        }

        return null;
    }

    public async Task<RestMessage?> GetMessageFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableMessage cache)
                    {
                        RestMessage? msg = await GetMessageAsync(cache.ChannelId, cache.MessageId);

                        if (msg != null)
                            SetVariable(block.GetVariableId(), msg);

                        return msg;
                    }

                    return obj as RestMessage;
                }

                break;
            case "data_message_current":
                {
                    if (Data.MessageCurrent == null)
                        return null;

                    if (Data.MessageCurrent.Item2 != null)
                        return Data.MessageCurrent.Item2;

                    RestMessage? msg = await GetMessageAsync(Data.MessageCurrent.Item1.ChannelId, Data.MessageCurrent.Item1.MessageId);
                    if (msg != null)
                        Data.MessageCurrent = Tuple.Create<DiscordCachableMessage, RestMessage?>(Data.MessageCurrent.Item1, msg);

                    return msg;
                }
            case "data_message_active":
                {
                    if (Data.MessageActive == null)
                        return null;

                    if (Data.MessageActive.Item2 != null)
                        return Data.MessageActive.Item2;

                    RestMessage? msg = await GetMessageAsync(Data.MessageActive.Item1.ChannelId, Data.MessageActive.Item1.MessageId);
                    if (msg != null)
                        Data.MessageActive = Tuple.Create<DiscordCachableMessage, RestMessage?>(Data.MessageActive.Item1, msg);

                    return msg;
                }
            case "data_message_get":
                {
                    RestChannel? Chan = null;
                    if (block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        Chan = await GetChannelFromBlock(chanBlock.block);

                    string MessageId = string.Empty;

                    if (Chan != null && !string.IsNullOrEmpty(MessageId) && ulong.TryParse(MessageId, out ulong id))
                    {
                        if (Cache.Messages.TryGetValue(id, out RestMessage? msg))
                            return msg;

                        if (Chan is IMessageChannel mc)
                            return await mc.GetMessageAsync(id) as RestMessage;

                    }


                }
                break;
        }

        return null;
    }

    public async Task<Tuple<RestWebhook?, string>?> GetWebhookFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return obj as Tuple<RestWebhook?, string>;
                break;
            case "data_category_active":
                return Data.WebhookActive;
            case "data_webhook_get_channel":
                {
                    string WebhookId = string.Empty;
                    if (block.inputs.TryGetValue("webhook_id", out WorkspaceBlockConnection? urlBlock) && urlBlock.block != null)
                        WebhookId = await GetStringFromBlock(urlBlock.block);

                    if (!string.IsNullOrEmpty(WebhookId) && ulong.TryParse(WebhookId, out ulong id))
                    {
                        if (Cache.Webhooks.TryGetValue(id, out Tuple<RestWebhook?, string> wh))
                            return wh;

                        return await GetWebhookAsync(id, null);
                    }
                }
                break;
            case "data_webhook_get_url":
                {
                    string Url = string.Empty;
                    if (block.inputs.TryGetValue("webhook_url", out WorkspaceBlockConnection? urlBlock) && urlBlock.block != null)
                        Url = await GetStringFromBlock(urlBlock.block);

                    if (!string.IsNullOrEmpty(Url))
                    {
                        string[] Split = Url.Split('/');
                        if (Split.Length == 7 && !string.IsNullOrEmpty(Split[5]) && ulong.TryParse(Split[5], out ulong id))
                        {
                            if (Cache.Webhooks.TryGetValue(id, out Tuple<RestWebhook?, string>? wh))
                                return wh;

                            return await GetWebhookAsync(id, Url);
                        }
                    }
                }
                break;
        }

        return null;
    }

    public DiscordWebhookClient GetWebhookClient(Tuple<RestWebhook?, string> webhook)
    {
        if (Cache.WebhookClients.TryGetValue(webhook.Item1.Id, out DiscordWebhookClient client))
            return client;

        DiscordWebhookClient cl = new DiscordWebhookClient(webhook.Item2, new DiscordRestConfig { APIOnRestInteractionCreation = false, DefaultRetryMode = RetryMode.RetryTimeouts });
        Cache.AddWebhookClient(webhook.Item1, cl);
        return cl;
    }

    public async Task<RestGuild?> GetServerFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableServer cache)
                    {
                        RestGuild? server = await GetGuildAsync(cache.ServerId);
                        if (server != null)
                            SetVariable(block.GetVariableId(), server);

                        return server;
                    }

                    return obj as RestGuild;
                }

                break;
            case "data_server_current":
                {
                    if (Data.ServerCurrent == null)
                        return null;

                    if (Data.ServerCurrent.Item2 != null)
                        return Data.ServerCurrent.Item2;

                    RestGuild? server = await GetGuildAsync(Data.ServerCurrent.Item1);
                    if (server != null)
                        Data.ServerCurrent = Tuple.Create<ulong, RestGuild?>(server.Id, server);

                    return server;
                }
            case "data_server_active":
                {
                    if (Data.ServerActive == null)
                        return null;

                    if (Data.ServerActive.Item2 != null)
                        return Data.ServerActive.Item2;



                    RestGuild? server = await GetGuildAsync(Data.ServerActive.Item1);
                    if (server != null)
                        Data.ServerActive = Tuple.Create<ulong, RestGuild?>(server.Id, server);

                    return server;

                }
            case "data_server_get":
                {
                    string ServerId = string.Empty;
                    if (block.inputs.TryGetValue("server_id", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                        ServerId = await GetStringFromBlock(srvBlock.block);

                    if (!string.IsNullOrEmpty(ServerId) && ulong.TryParse(ServerId, out ulong id))
                    {
                        if (Cache.Servers.TryGetValue(id, out RestGuild server))
                            return server;

                        return await GetGuildAsync(id);
                    }
                }
                break;
        }

        return null;
    }

    public async Task<ulong?> GetServerIdFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj) && obj is RestGuild rg)
                {
                    if (obj is DiscordCachableServer cache)
                        return cache.ServerId;

                    if (obj is RestGuild g)
                        return g.Id;
                }
                break;
            case "data_server_current":
                {
                    if (Data.ServerCurrent != null)
                        return Data.ServerCurrent.Item1;
                }
                break;
            case "data_server_active":
                {
                    if (Data.ServerActive != null)
                        return Data.ServerActive.Item1;
                }
                break;
            case "data_server_get":
                {
                    string ServerId = string.Empty;
                    if (block.inputs.TryGetValue("server_id", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                        ServerId = await GetStringFromBlock(srvBlock.block);

                    if (!string.IsNullOrEmpty(ServerId) && ulong.TryParse(ServerId, out ulong id))
                        return id;
                }
                break;
        }

        return null;
    }

    public async Task<Tuple<Tuple<ulong, RestGuild?>?, IEmote>?> GetEmojiFromBlockAsync(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    //if (obj is DiscordCachableEmoji cache)
                    //    return cache;

                    return obj as Tuple<Tuple<ulong, RestGuild?>?, IEmote>;
                }
                break;
            case "data_emoji_active":
                return Data.EmojiActive;
            case "data_emoji_get":
                RestGuild? Server = null;
                if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                    Server = await GetServerFromBlock(srvBlock.block);

                string EmojiId = string.Empty;
                if (block.inputs.TryGetValue("emoji_id", out WorkspaceBlockConnection? emojiBlock) && emojiBlock.block != null)
                    EmojiId = await GetStringFromBlock(emojiBlock.block);

                if (Server != null && !string.IsNullOrEmpty(EmojiId) && ulong.TryParse(EmojiId, out ulong id))
                {
                    if (Cache.Emojis.TryGetValue(id, out Tuple<RestGuild, GuildEmote>? ge))
                    {
                        if (ge.Item1.Id != Server.Id)
                            return null;

                        return Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(Tuple.Create<ulong, RestGuild?>(Server.Id, Server), ge.Item2);
                    }
                    GuildEmote Emote = await Server.GetEmoteAsync(id);

                    if (Emote != null)
                    {
                        Cache.AddEmoji(Server, Emote);
                        return Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(Tuple.Create<ulong, RestGuild?>(Server.Id, Server), Emote);
                    }


                }
                break;
            case "data_emoji_parse":
                string EmojiString = string.Empty;
                if (block.inputs.TryGetValue("emoji_string", out WorkspaceBlockConnection? emojiBlock2) && emojiBlock2.block != null)
                    EmojiString = await GetStringFromBlock(emojiBlock2.block);

                if (string.IsNullOrEmpty(EmojiString))
                    return null;

                if (Emote.TryParse(EmojiString, out Emote emote))
                    return Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(null, emote);

                if (Emoji.TryParse(EmojiString, out Emoji emo))
                    return Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(null, emo);

                break;
        }

        return null;
    }

    public async Task<RestRole?> GetRoleFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableRole cache)
                    {
                        RestRole? role = await GetRoleAsync(cache.serverId, cache.roleId);
                        if (role != null)
                            SetVariable(block.GetVariableId(), role);

                        return role;
                    }

                    return obj as RestRole;
                }
                break;
            case "data_role_active":
                {
                    if (Data.RoleActive == null)
                        return null;

                    if (Data.RoleActive.Item2 != null)
                        return Data.RoleActive.Item2;

                    RestRole? role = await GetRoleAsync(Data.RoleActive.Item1.serverId, Data.RoleActive.Item1.roleId);
                    if (role != null)
                        Data.RoleActive = Tuple.Create<DiscordCachableRole, RestRole?>(Data.RoleActive.Item1, role);

                    return role;
                }
            case "data_role_get":
                {
                    RestGuild? Server = null;
                    if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                        Server = await GetServerFromBlock(srvBlock.block);

                    string RoleId = string.Empty;
                    if (block.inputs.TryGetValue("role_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        RoleId = await GetStringFromBlock(roleBlock.block);

                    if (Server != null && !string.IsNullOrEmpty(RoleId) && ulong.TryParse(RoleId, out ulong id))
                    {
                        return Server.GetRole(id);
                    }
                }
                break;
        }

        return null;
    }

    public async Task<ulong?> GetRoleIdFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableRole cache)
                        return cache.roleId;

                    if (obj is RestRole r)
                        return r.Id;
                }
                break;
            case "data_role_active":
                {
                    if (Data.RoleActive != null)
                        return Data.RoleActive.Item1.roleId;
                }
                break;
            case "data_role_get":
                {
                    string RoleId = string.Empty;
                    if (block.inputs.TryGetValue("role_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        RoleId = await GetStringFromBlock(roleBlock.block);

                    if (!string.IsNullOrEmpty(RoleId) && ulong.TryParse(RoleId, out ulong id))
                    {
                        return id;
                    }
                }
                break;
        }

        return null;
    }

    public async Task<RestGuildUser?> GetMemberFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableMember cache)
                    {
                        RestGuildUser? member = await GetMemberAsync(cache.serverId, cache.memberId);
                        if (member != null)
                            SetVariable(block.GetVariableId(), member);

                        return member;
                    }

                    return obj as RestGuildUser;
                }
                break;
            case "data_member_app":
                {
                    if (Data.ServerCurrent == null || Data.MemberApp == null)
                        return null;

                    if (Data.MemberApp.Item2 != null)
                        return Data.MemberApp.Item2;

                    RestGuildUser? member = await GetMemberAsync(Data.ServerCurrent.Item1, Client.CurrentUser.Id);
                    if (member != null)
                        Data.MemberApp = Tuple.Create<ulong, RestGuildUser?>(member.Id, member);

                    return member;
                }
            case "data_member_get":
                {

                    RestGuild? Server = null;
                    if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                        Server = await GetServerFromBlock(srvBlock.block);

                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        UserId = await GetStringFromBlock(roleBlock.block);

                    if (Server != null && !string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                    {
                        if (Cache.Members.TryGetValue($"{Server.Id}-{UserId}", out RestGuildUser? mb))
                            return mb;

                        return await GetMemberAsync(Server.Id, id);
                    }

                }
                break;
            case "data_member_current":
                return Data.MemberCurrent;
            case "data_member_active":
                {
                    if (Data.MemberActive == null)
                        return null;

                    if (Data.MemberActive.Item2 != null)
                        return Data.MemberActive.Item2;

                    RestGuildUser? member = await GetMemberAsync(Data.MemberActive.Item1.serverId, Data.MemberActive.Item1.memberId);
                    if (member != null)
                        Data.MemberActive = Tuple.Create<DiscordCachableMember, RestGuildUser?>(Data.MemberActive.Item1, member);

                    return member;
                }
        }

        return null;
    }

    public async Task<ulong?> GetMemberIdFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableMember cache)
                        return cache.memberId;

                    if (obj is RestGuildUser member)
                        return member.Id;
                }
                break;
            case "data_member_app":
                {
                    if (Data.ServerCurrent != null)
                        return Client.CurrentUser.Id;

                    return null;
                }
            case "data_member_get":
                {
                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        UserId = await GetStringFromBlock(roleBlock.block);

                    if (!string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                        return id;
                }
                break;
            case "data_member_current":
                return Data.MemberCurrent?.Id;
            case "data_member_active":
                {
                    if (Data.MemberActive != null)
                        return Data.MemberActive.Item1.memberId;
                }
                break;
        }

        return null;
    }

    public async Task<RestUser?> GetUserFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableUser cache)
                    {
                        RestUser? user = await GetUserAsync(cache.UserId);

                        if (user != null)
                            SetVariable(block.GetVariableId(), user);

                        return user;
                    }
                    else if (obj is DiscordCachableMember cachem)
                    {
                        RestGuildUser? member = await GetMemberAsync(cachem.serverId, cachem.memberId);
                        if (member != null)
                            SetVariable(block.GetVariableId(), member);

                        return member;
                    }

                    return obj as RestUser;
                }

                break;
            case "data_user_app":
                return Client.CurrentUser;
            case "data_user_current":
                return Data.UserCurrent;
            case "data_user_active":
                {
                    if (Data.UserActive == null)
                        return null;

                    if (Data.UserActive.Item2 != null)
                        return Data.UserActive.Item2;

                    RestUser? user = await GetUserAsync(Data.UserActive.Item1);
                    if (user != null)
                        Data.UserActive = Tuple.Create<ulong, RestUser?>(user.Id, user);

                    return user;
                }
            case "data_member_current":
            case "data_member_active":
            case "data_member_get":
            case "data_member_app":
                return await GetMemberFromBlock(block);
            case "data_user_get":
                {
                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        UserId = await GetStringFromBlock(roleBlock.block);

                    if (!string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                    {
                        if (Cache.Users.TryGetValue(id, out RestUser? us))
                            return us;

                        return await GetUserAsync(id);
                    }
                }
                break;
        }

        return null;
    }

    public async Task<ulong?> GetUserIdFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is DiscordCachableUser cache)
                        return cache.UserId;

                    if (obj is RestUser u)
                        return u.Id;
                }
                break;
            case "data_user_app":
                return Client.CurrentUser.Id;
            case "data_user_current":
                return Data.UserCurrent?.Id;
            case "data_user_active":
                {
                    if (Data.UserActive == null)
                        return null;

                    return Data.UserActive.Item1;

                }
            case "data_member_current":
            case "data_member_active":
            case "data_member_get":
            case "data_member_app":
                return await GetMemberIdFromBlock(block);
            case "data_user_get":
                {
                    string UserId = string.Empty;
                    if (block.inputs.TryGetValue("user_id", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        UserId = await GetStringFromBlock(roleBlock.block);

                    if (!string.IsNullOrEmpty(UserId) && ulong.TryParse(UserId, out ulong id))
                        return id;
                }
                break;
        }

        return null;
    }

    public async Task<Color?> GetColorFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                {
                    if (obj is string hex && !string.IsNullOrEmpty(hex) && hex.StartsWith('#'))
                    {
                        try
                        {
                            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex);
                            int r = Convert.ToInt16(color.R);
                            int g = Convert.ToInt16(color.G);
                            int b = Convert.ToInt16(color.B);
                            return new Color(r, g, b);
                        }
                        catch { }
                    }

                    return obj as Color?;
                }
                break;
            case "color_hex_picker":
                {
                    try
                    {
                        System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(block.fields["custom_color"].ToString());
                        int r = Convert.ToInt16(color.R);
                        int g = Convert.ToInt16(color.G);
                        int b = Convert.ToInt16(color.B);
                        return new Color(r, g, b);
                    }
                    catch { }
                }
                break;
            case "color_hex":
                {
                    if (block.inputs.TryGetValue("hex", out WorkspaceBlockConnection hexStringBlock) && hexStringBlock.block != null)
                    {
                        string Hex = await GetStringFromBlock(hexStringBlock.block);
                        if (!string.IsNullOrEmpty(Hex) && Hex.StartsWith("#"))
                        {
                            try
                            {
                                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(Hex);
                                int r = Convert.ToInt16(color.R);
                                int g = Convert.ToInt16(color.G);
                                int b = Convert.ToInt16(color.B);
                                return new Color(r, g, b);
                            }
                            catch { }
                        }
                    }
                }
                break;
            case "color_rgb":
                {
                    int r = block.inputs["red"].block.fields["NUM"].ToObject<int>();
                    int g = block.inputs["green"].block.fields["NUM"].ToObject<int>();
                    int b = block.inputs["blue"].block.fields["NUM"].ToObject<int>();
                    return new Color(r, g, b);
                }
        }

        return null;
    }

    public void SetMessageData(WorkspaceBlock block, RestMessage message)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, message);
                break;
            case "data_message_active":
                Data.MessageActive = Tuple.Create<DiscordCachableMessage, RestMessage?>(new DiscordCachableMessage(message.Channel.Id, message.Id), message);
                break;
        }
    }

    public void SetMessageData(WorkspaceBlock block, ulong channel, ulong message)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableMessage(channel, message));
                break;
            case "data_message_active":
                Data.MessageActive = Tuple.Create<DiscordCachableMessage, RestMessage?>(new DiscordCachableMessage(channel, message), null);
                break;
        }
    }

    public void SetChannelData(WorkspaceBlock block, ulong channel)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableChannel(channel));
                break;
            case "data_channel_active":
                Data.ChannelActive = Tuple.Create<ulong, RestChannel?>(channel, null);
                break;
        }
    }

    public void SetChannelData(WorkspaceBlock block, RestChannel channel)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, channel);
                break;
            case "data_channel_active":
                Data.ChannelActive = Tuple.Create<ulong, RestChannel?>(channel.Id, channel);
                break;
        }
    }

    public void SetCategoryData(WorkspaceBlock block, ulong category)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableCategory(category));
                break;
            case "data_category_active":
                Data.CategoryActive = Tuple.Create<ulong, RestCategoryChannel?>(category, null);
                break;
        }
    }

    public void SetCategoryData(WorkspaceBlock block, RestCategoryChannel category)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, category);
                break;
            case "data_category_active":
                Data.CategoryActive = Tuple.Create<ulong, RestCategoryChannel?>(category.Id, category);
                break;
        }
    }

    public void SetWebhookData(WorkspaceBlock block, Tuple<RestWebhook?, string> webhook)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, webhook);
                break;
            case "data_webhook_active":
                Data.WebhookActive = webhook;
                break;
        }
    }

    public void SetServerData(WorkspaceBlock block, ulong server)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableServer(server));
                break;
            case "data_file_active":
                Data.ServerActive = Tuple.Create<ulong, RestGuild?>(server, null);
                break;
        }
    }

    public void SetServerData(WorkspaceBlock block, RestGuild server)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, server);
                break;
            case "data_file_active":
                Data.ServerActive = Tuple.Create<ulong, RestGuild?>(server.Id, server);
                break;
        }
    }

    public void SetEmojiData(WorkspaceBlock block, ulong guild, IEmote emoji)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableEmoji(guild, emoji.Name));
                break;
            case "data_emoji_active":

                Data.EmojiActive = Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(Tuple.Create<ulong, RestGuild?>(guild, null), emoji);
                break;
        }
    }

    public void SetEmojiData(WorkspaceBlock block, RestGuild guild, GuildEmote emoji)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, emoji);
                break;
            case "data_emoji_active":
                Data.EmojiActive = Tuple.Create<Tuple<ulong, RestGuild?>?, IEmote>(Tuple.Create<ulong, RestGuild?>(guild.Id, guild), emoji);
                break;
        }
    }

    public void SetRoleData(WorkspaceBlock block, ulong serverId, ulong role)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableRole(serverId, role));
                break;
            case "data_role_active":
                Data.RoleActive = Tuple.Create<DiscordCachableRole, RestRole?>(new DiscordCachableRole(serverId, role), null);
                break;
        }
    }

    public void SetRoleData(WorkspaceBlock block, ulong serverId, RestRole role)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, role);
                break;
            case "data_role_active":
                Data.RoleActive = Tuple.Create<DiscordCachableRole, RestRole?>(new DiscordCachableRole(serverId, role.Id), role);
                break;
        }
    }

    public void SetMemberData(WorkspaceBlock block, ulong server, ulong member)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableMember(server, member));
                break;
            case "data_member_active":
                Data.MemberActive = Tuple.Create<DiscordCachableMember, RestGuildUser?>(new DiscordCachableMember(server, member), null);
                break;
        }
    }

    public void SetMemberData(WorkspaceBlock block, RestGuildUser member)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, member);
                break;
            case "data_member_active":
                Data.MemberActive = Tuple.Create<DiscordCachableMember, RestGuildUser?>(new DiscordCachableMember(member.GuildId, member.Id), member);
                break;
        }
    }

    public void SetUserData(WorkspaceBlock block, ulong user)
    {
        Console.WriteLine("Set User Data: " + user);
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, new DiscordCachableUser(user));
                break;
            case "data_user_active":
                Data.UserActive = Tuple.Create<ulong, RestUser?>(user, null);
                break;
        }
    }

    public void SetUserData(WorkspaceBlock block, RestUser user)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, user);
                break;
            case "data_user_active":
                Data.UserActive = Tuple.Create<ulong, RestUser?>(user.Id, user);
                break;
        }
    }


    public void CopyModalData(WorkspaceBlock copyBlockType, WorkspaceBlock toBlockType)
    {
        object? obj = GetDynamicData(copyBlockType);

        switch (toBlockType.type)
        {
            case "variables_get":
                Console.WriteLine("REEE");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj.ToString()));
                SetVariable(toBlockType.GetVariableId(), obj);
                break;
            case "data_message_current":
                {
                    if (obj == null)
                        Data.MessageCurrent = null;
                    else if (obj is RestMessage msg)
                        Data.MessageCurrent = msg.ToCachableTuple();
                    else if (obj is DiscordCachableMessage cmsg)
                        Data.MessageCurrent = cmsg.ToCachableTuple();
                }
                break;
            case "data_message_active":
                {
                    if (obj == null)
                        Data.MessageActive = null;
                    else if (obj is RestMessage msg)
                        Data.MessageActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableMessage cmsg)
                        Data.MessageActive = cmsg.ToCachableTuple();
                }
                break;

            case "data_channel_active":
                {
                    if (obj == null)
                        Data.ChannelActive = null;
                    else if (obj is RestChannel msg)
                        Data.ChannelActive = msg.ToCachableTupleId();
                    else if (obj is DiscordCachableChannel cmsg)
                        Data.ChannelActive = cmsg.ToCachableTuple();
                    else if (obj is ulong umsg)
                        Data.ChannelActive = umsg.ToChannelCachableTuple();
                }
                break;

            case "data_category_active":
                {
                    if (obj == null)
                        Data.CategoryActive = null;
                    else if (obj is RestCategoryChannel msg)
                        Data.CategoryActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableChannel cmsg)
                        Data.CategoryActive = cmsg.ToCachableCatTuple();
                    else if (obj is ulong umsg)
                        Data.CategoryActive = umsg.ToCatCachableTuple();
                }
                break;

            case "data_server_active":
                {
                    if (obj == null)
                        Data.ServerActive = null;
                    else if (obj is RestGuild msg)
                        Data.ServerActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableServer cmsg)
                        Data.ServerActive = cmsg.ToCachableTuple();
                    else if (obj is ulong umsg)
                        Data.ServerActive = umsg.ToServerCachableTuple();
                }
                break;

            case "data_role_active":
                {
                    if (obj == null)
                        Data.RoleActive = null;
                    else if (obj is RestRole msg)
                        Data.RoleActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableRole cmsg)
                        Data.RoleActive = cmsg.ToCachableTuple();
                }
                break;

            case "data_member_active":
                {
                    if (obj == null)
                        Data.MemberActive = null;
                    else if (obj is RestGuildUser msg)
                        Data.MemberActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableMember cmsg)
                        Data.MemberActive = cmsg.ToCachableTuple();
                }
                break;

            case "data_user_active":
                {

                    if (obj == null)
                        Data.UserActive = null;
                    else if (obj is RestUser msg)
                        Data.UserActive = msg.ToCachableTuple();
                    else if (obj is DiscordCachableUser cmsg)
                        Data.UserActive = cmsg.ToCachableTuple();
                    else if (obj is ulong umsg)
                        Data.UserActive = umsg.ToUserCachableTuple();
                }
                break;
        }
    }

    private object GetDynamicData(WorkspaceBlock blockType)
    {
        switch (blockType.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(blockType.GetVariableId(), out object obj))
                    return obj;
                break;
            case "data_message_current":
                {
                    if (Data.MessageCurrent != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"ms{Data.MessageCurrent.Item1.ChannelId}-{Data.MessageCurrent.Item1.MessageId}";

                        if (Data.MessageCurrent.Item2 != null)
                            return Data.MessageCurrent.Item2;

                        return Data.MessageCurrent.Item1;
                    }
                }
                break;
            case "data_message_active":
                {
                    if (Data.MessageActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"ms{Data.MessageActive.Item1.ChannelId}-{Data.MessageActive.Item1.MessageId}";

                        if (Data.MessageActive.Item2 != null)
                            return Data.MessageActive.Item2;

                        return Data.MessageActive.Item1;
                    }
                }
                break;

            case "data_channel_active":
                {
                    if (Data.ChannelActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"ch{Data.ChannelActive.Item1}";

                        if (Data.ChannelActive.Item2 != null)
                            return Data.ChannelActive.Item2;

                        return Data.ChannelActive.Item1;
                    }
                }
                break;

            case "data_category_active":
                {
                    if (Data.CategoryActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"ct{Data.CategoryActive.Item1}";

                        if (Data.CategoryActive.Item2 != null)
                            return Data.CategoryActive.Item2;

                        return Data.CategoryActive.Item1;
                    }
                }
                break;

            case "data_server_active":
                {
                    if (Data.ServerActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"sv{Data.ServerActive.Item1}";

                        if (Data.ServerActive.Item2 != null)
                            return Data.ServerActive.Item2;

                        return Data.ServerActive.Item1;
                    }
                }
                break;

            case "data_role_active":
                {
                    if (Data.RoleActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"rl{Data.RoleActive.Item1.serverId}-{Data.RoleActive.Item1.roleId}";

                        if (Data.RoleActive.Item2 != null)
                            return Data.RoleActive.Item2;

                        return Data.RoleActive.Item1;
                    }
                }
                break;

            case "data_member_active":
                {
                    if (Data.MemberActive != null)
                    {
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"mb{Data.MemberActive.Item1.serverId}-{Data.MemberActive.Item1.memberId}";

                        if (Data.MemberActive.Item2 != null)
                            return Data.MemberActive.Item2;

                        return Data.MemberActive.Item1;
                    }
                }
                break;

            case "data_user_active":
                {
                    Console.WriteLine("Check Active");
                    if (Data.UserActive != null)
                    {
                        Console.WriteLine("Set Active User Data: " + Data.UserActive.Item1);
                        if (string.IsNullOrEmpty(ModalDataType))
                            ModalDataType = $"ur{Data.UserActive.Item1}";

                        if (Data.UserActive.Item2 != null)
                            return Data.UserActive.Item2;

                        return Data.UserActive.Item1;
                    }
                }
                break;
        }

        return '.';
    }
}