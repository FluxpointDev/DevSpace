using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public class DiscordData
{
    public Tuple<DiscordCachableMessage, RestMessage?>? MessageCurrent { get; set; } = null;
    public Tuple<DiscordCachableMessage, RestMessage?>? MessageActive { get; set; } = null;


    public RestChannel? ChannelCurrent { get; set; } = null;
    public Tuple<ulong, RestChannel?>? ChannelActive { get; set; } = null;

    public Tuple<ulong, RestCategoryChannel?>? CategoryCurrent { get; set; } = null;
    public Tuple<ulong, RestCategoryChannel?>? CategoryActive { get; set; } = null;

    public Tuple<RestWebhook?, string>? WebhookActive { get; set; } = null;

    public Tuple<ulong, RestGuild?>? ServerCurrent { get; set; } = null;
    public Tuple<ulong, RestGuild?>? ServerActive { get; set; } = null;

    public Tuple<Tuple<ulong, RestGuild?>?, IEmote>? EmojiActive { get; set; } = null;

    public Tuple<DiscordCachableRole, RestRole?>? RoleActive { get; set; } = null;

    public Tuple<ulong, RestGuildUser?>? MemberApp { get; set; } = null;
    public RestGuildUser? MemberCurrent { get; set; } = null;
    public Tuple<DiscordCachableMember, RestGuildUser?>? MemberActive { get; set; } = null;

    public RestUser UserApp { get; set; } = null!;
    public RestUser? UserCurrent { get; set; } = null;
    public Tuple<ulong, RestUser?>? UserActive { get; set; } = null;

    public GuildPermissions? PermissionsActive { get; set; } = null;
}