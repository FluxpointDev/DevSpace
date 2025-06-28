using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using Discord;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Apps.Data;

public class AppData : ITeamResource
{
    public AppData() : base(ResourceType.App) { }

    public string Description { get; set; }

    public string AvatarUrl { get; set; }

    //public string GetIconOrDefault()
    //{
    //    if (string.IsNullOrEmpty(Icon))
    //        return "https://cdn.discordapp.com/embed/avatars/0.png";

    //    return Icon;
    //}

    public DiscordAppData DiscordData { get; set; } = new DiscordAppData();

    public AppType AppType { get; set; }

    public string BotId { get; set; }
    public string BotOwnerId { get; set; }
    public string[] BotDeveloperIds = [];

    public Dictionary<string, DiscordAppSlashCommand> SlashCommands = new Dictionary<string, DiscordAppSlashCommand>();
    public Dictionary<string, IDiscordAppCommand> UserCommands = new Dictionary<string, IDiscordAppCommand>();
    public Dictionary<string, IDiscordAppCommand> MessageCommands = new Dictionary<string, IDiscordAppCommand>();

    public Dictionary<string, IDiscordAppInteraction> ButtonInteractions = new Dictionary<string, IDiscordAppInteraction>();
    public Dictionary<string, IDiscordAppInteraction> SelectInteractions = new Dictionary<string, IDiscordAppInteraction>();
    public Dictionary<string, IDiscordAppInteraction> ModalInteractions = new Dictionary<string, IDiscordAppInteraction>();

    public Dictionary<string, DiscordAppServerCommands> ServerCommands = new Dictionary<string, DiscordAppServerCommands>();

    public string TermsOfServiceUrl { get; set; }
    public string PrivacyPolicyUrl { get; set; }

    public string EncryptedToken { get; set; }
    public string EncryptedInteractionKey { get; set; }

    private string DecryptedInteractionKey { get; set; }

    //public void UpdateDecryptedInteractionKey()
    //{
    //    DecryptedInteractionKey = Crypt.DecryptString(EncryptedInteractionKey);
    //}

    //public string GetDecryptedInteractionKey()
    //{
    //    if (string.IsNullOrEmpty(DecryptedInteractionKey))
    //        DecryptedInteractionKey = Crypt.DecryptString(EncryptedInteractionKey);

    //    return DecryptedInteractionKey;
    //}

    public bool GetServerCommands(string key, out DiscordAppServerCommands? commands)
    {
        if (ServerCommands.TryGetValue(key, out commands))
            return true;

        commands = new DiscordAppServerCommands();

        return true;
    }

    private AppEvents Events = new AppEvents();
    private AppCache Cache = new AppCache();
    public AppCache GetCache()
    {
        return Cache;
    }
    public AppEvents GetEvents()
    {
        return Events;
    }

    public string GetBotInviteUrl(string permOverride = "")
    {
        if (DiscordData.IsPublicApp)
            return "https://discord.com/oauth2/authorize?client_id=" + BotId;
        else
            return "https://discord.com/oauth2/authorize?client_id=" + BotId + "&scope=bot+applications.commands&permissions=" + (string.IsNullOrEmpty(permOverride) ? DiscordData.GuildPermissions : permOverride);
    }

    public string GetBotInteractionsUrl()
    {
        if (Program.IsDevMode)
            return "https://novanode.dev/test/interactions/" + Id.ToString();
        return "https://" + _Data.Config.Instance.PublicDomain + "/interactions/" + Id.ToString();
    }

    public class AppEvents
    {
        public delegate void saisieDateTime(string title, object data);
        public event saisieDateTime LogEvent;

        public void TriggerEvent(string title, object data)
        {
            LogEvent?.Invoke(title, data);
        }
    }


    public async Task<UpdateResult> UpdateAsync(UpdateDefinition<AppData> update, Action? action = null)
    {
        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Apps.Collection.UpdateOneAsync(filter, update);
        if (action != null && Result.IsAcknowledged)
            action?.Invoke();
        return Result;
    }
}

public class AppCache
{
    private Dictionary<ObjectId, RequestBlocks_Block> CommandCache = new Dictionary<ObjectId, RequestBlocks_Block>();
    public void AddCommandCache(ObjectId workspace, RequestBlocks_Block block)
    {
        if (CommandCache.ContainsKey(workspace))
            CommandCache[workspace] = block;
        else
            CommandCache.Add(workspace, block);
    }

    public void RemoveCommandCache(ObjectId workspace)
    {
        CommandCache.Remove(workspace);
    }

    public RequestBlocks_Block? GetCacheCommand(ObjectId workspace)
    {
        if (CommandCache.TryGetValue(workspace, out RequestBlocks_Block? block))
            return block;

        return null;
    }
}

public class DiscordAppServerCommands
{
    public Dictionary<string, DiscordAppSlashCommand> SlashCommands = new Dictionary<string, DiscordAppSlashCommand>();
    public Dictionary<string, IDiscordAppCommand> UserCommands = new Dictionary<string, IDiscordAppCommand>();
    public Dictionary<string, IDiscordAppCommand> MessageCommands = new Dictionary<string, IDiscordAppCommand>();
}
public class IDiscordAppInteraction
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public bool IsEnabled { get; set; }

    public ObjectId? WorkspaceId { get; set; }

    public string? OpenModal { get; set; }

    public string? ModalDataType { get; set; }

    public bool IsEphemeral { get; set; }
}
public class IDiscordAppCommand : IDiscordAppInteraction
{
    public string CommandId { get; set; }
    public bool AllowUserApps { get; set; }
}
public class DiscordAppSlashCommand : IDiscordAppCommand
{
    public bool IsMainGroup { get; set; }

    public string? InputHash { get; set; }

    public Dictionary<string, DiscordAppSlashCommand> Commands = new Dictionary<string, DiscordAppSlashCommand>();

    public DiscordAppCommandInput[] Inputs = [];

    public Dictionary<string, AppSubGroup> SubGroups = new Dictionary<string, AppSubGroup>();

    public SlashCommandProperties Build()
    {
        SlashCommandBuilder builder = new SlashCommandBuilder
        {
            Name = Name,
            Description = Description ?? "‎ ",
        };

        if (Inputs.Any())
        {
            foreach (DiscordAppCommandInput i in Inputs)
            {
                ApplicationCommandOptionType Type = ApplicationCommandOptionType.String;
                bool IsLengthInput = false;
                List<ApplicationCommandOptionChoiceProperties>? choices = null;
                switch (i.Type)
                {
                    case DiscordAppCommandInputType.Attachment:
                        Type = ApplicationCommandOptionType.Attachment;
                        break;
                    case DiscordAppCommandInputType.Boolean:
                        Type = ApplicationCommandOptionType.Boolean;
                        break;
                    case DiscordAppCommandInputType.Channel:
                        Type = ApplicationCommandOptionType.Channel;
                        break;
                    case DiscordAppCommandInputType.Integer:
                        Type = ApplicationCommandOptionType.Integer;
                        break;
                    case DiscordAppCommandInputType.Member:
                        Type = ApplicationCommandOptionType.User;
                        break;
                    case DiscordAppCommandInputType.Mentionable:
                        Type = ApplicationCommandOptionType.Mentionable;
                        break;
                    case DiscordAppCommandInputType.Number:
                        Type = ApplicationCommandOptionType.Number;
                        break;
                    case DiscordAppCommandInputType.Role:
                        Type = ApplicationCommandOptionType.Role;
                        break;
                    case DiscordAppCommandInputType.String:
                        IsLengthInput = true;
                        break;
                    case DiscordAppCommandInputType.User:
                        Type = ApplicationCommandOptionType.User;
                        break;
                }
                if (i.Choices != null && i.Choices.Count != 0)
                {
                    choices = new List<ApplicationCommandOptionChoiceProperties>();
                    foreach (KeyValuePair<string, string> c in i.Choices)
                    {
                        choices.Add(new ApplicationCommandOptionChoiceProperties() { Name = c.Key, Value = c.Value });
                    }
                }
                builder.AddOption(i.Name, Type, i.Description ?? "‎ ", i.IsRequired, null, false,
                    !IsLengthInput && i.Min != 0 ? i.Min : null, !IsLengthInput && i.Max != 0 ? i.Max : null,
                    null, null, minLength: IsLengthInput && i.Min != 0 ? (int)i.Min : null, maxLength: IsLengthInput && i.Max != 0 ? (int)i.Max : null, choices: choices != null ? choices.ToArray() : null);
            }
        }

        if (IsMainGroup)
        {
            foreach (DiscordAppSlashCommand i in Commands.Values)
            {
                SlashCommandOptionBuilder build = new SlashCommandOptionBuilder().WithName(i.Name).WithType(ApplicationCommandOptionType.SubCommand);
                if (!string.IsNullOrEmpty(i.Description))
                    build = build.WithDescription(i.Description);

                builder = builder.AddOption(build);
            }

            foreach (AppSubGroup s in SubGroups.Values)
            {
                SlashCommandOptionBuilder Opts = new SlashCommandOptionBuilder().WithName(s.Name).WithType(ApplicationCommandOptionType.SubCommandGroup);
                if (!string.IsNullOrEmpty(s.Description))
                    Opts = Opts.WithDescription(s.Description);

                foreach (DiscordAppSlashCommand i in s.Commands.Values)
                {
                    SlashCommandOptionBuilder builder2 = new SlashCommandOptionBuilder().WithName(i.Name).WithType(ApplicationCommandOptionType.SubCommand);
                    if (!string.IsNullOrEmpty(i.Description))
                        builder2 = builder2.WithDescription(i.Description);
                    Opts.AddOption(builder2);
                }

                if (Opts.Options != null && Opts.Options.Count != 0)
                    builder = builder.AddOption(Opts);
            }
        }

        return builder.Build();
    }
}
public class AppSubGroup
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, DiscordAppSlashCommand> Commands = new Dictionary<string, DiscordAppSlashCommand>();
}

public class DiscordAppCommandInput
{
    public DiscordAppCommandInputType Type { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public Dictionary<string, string> Choices { get; set; }
}
public enum DiscordAppCommandInputType
{
    String, Integer, Boolean, User, Member, Channel, Role, Mentionable, Number, Attachment
}

public class DiscordAppData
{
    public bool IsGuildInstall;
    public ulong GuildPermissions = 0;
    public bool IsUserInstall;
    public bool IsPublicApp;
}
public enum AppType
{
    Discord
}