using DevSpaceWeb.Apps.Runtime.DiscordApp;
using Discord.Rest;

namespace DevSpaceWeb.Apps;

public static class DiscordRuntimeExtensions
{
    public static DiscordCachableMessage ToCachable(this RestMessage msg)
    {
        return new DiscordCachableMessage(msg.Channel.Id, msg.Id);
    }

    public static Tuple<DiscordCachableMessage, RestMessage?> ToCachableTuple(this RestMessage msg)
    {
        return Tuple.Create<DiscordCachableMessage, RestMessage?>(msg.ToCachable(), msg);
    }

    public static Tuple<DiscordCachableMessage, RestMessage?> ToCachableTuple(this DiscordCachableMessage msg)
    {
        return Tuple.Create<DiscordCachableMessage, RestMessage?>(msg, null);
    }

    public static DiscordCachableUser ToCachable(this RestUser user)
    {
        return new DiscordCachableUser(user.Id);
    }

    public static Tuple<ulong, RestUser?> ToCachableTuple(this RestUser user)
    {
        return Tuple.Create<ulong, RestUser?>(user.Id, user);
    }

    public static Tuple<ulong, RestUser?> ToCachableTuple(this DiscordCachableUser user)
    {
        return Tuple.Create<ulong, RestUser?>(user.UserId, null);
    }

    public static Tuple<ulong, RestUser?> ToUserCachableTuple(this ulong user)
    {
        return Tuple.Create<ulong, RestUser?>(user, null);
    }

    public static DiscordCachableMember ToCachable(this RestGuildUser member)
    {
        return new DiscordCachableMember(member.GuildId, member.Id);
    }

    public static Tuple<DiscordCachableMember, RestGuildUser?> ToCachableTuple(this RestGuildUser member)
    {
        return Tuple.Create<DiscordCachableMember, RestGuildUser?>(member.ToCachable(), member);
    }

    public static Tuple<DiscordCachableMember, RestGuildUser?> ToCachableTuple(this DiscordCachableMember member)
    {
        return Tuple.Create<DiscordCachableMember, RestGuildUser?>(member, null);
    }

    public static DiscordCachableChannel ToCachable(this RestChannel chan)
    {
        return new DiscordCachableChannel(chan.Id);
    }

    public static Tuple<ulong, RestChannel?> ToCachableTupleId(this RestChannel chan)
    {
        return Tuple.Create<ulong, RestChannel?>(chan.Id, chan);
    }

    public static Tuple<ulong, RestChannel?> ToChannelCachableTuple(this ulong chan)
    {
        return Tuple.Create<ulong, RestChannel?>(chan, null);
    }

    public static Tuple<DiscordCachableChannel, RestChannel?> ToCachableTuple(this RestChannel chan)
    {
        return Tuple.Create<DiscordCachableChannel, RestChannel?>(chan.ToCachable(), chan);
    }

    public static Tuple<ulong, RestChannel?> ToCachableTuple(this DiscordCachableChannel chan)
    {
        return Tuple.Create<ulong, RestChannel?>(chan.ChannelId, null);
    }

    public static DiscordCachableChannel ToCachable(this RestCategoryChannel chan)
    {
        return new DiscordCachableChannel(chan.Id);
    }

    public static Tuple<ulong, RestCategoryChannel?> ToCachableTuple(this RestCategoryChannel chan)
    {
        return Tuple.Create<ulong, RestCategoryChannel?>(chan.Id, chan);
    }

    public static Tuple<ulong, RestCategoryChannel?> ToCachableCatTuple(this DiscordCachableChannel chan)
    {
        return Tuple.Create<ulong, RestCategoryChannel?>(chan.ChannelId, null);
    }

    public static Tuple<ulong, RestCategoryChannel?> ToCatCachableTuple(this ulong chan)
    {
        return Tuple.Create<ulong, RestCategoryChannel?>(chan, null);
    }

    public static DiscordCachableServer ToCachable(this RestGuild guild)
    {
        return new DiscordCachableServer(guild.Id);
    }

    public static Tuple<ulong, RestGuild?> ToCachableTuple(this RestGuild server)
    {
        return Tuple.Create<ulong, RestGuild?>(server.Id, server);
    }

    public static Tuple<ulong, RestGuild?> ToCachableTuple(this DiscordCachableServer server)
    {
        return Tuple.Create<ulong, RestGuild?>(server.ServerId, null);
    }

    public static Tuple<ulong, RestGuild?> ToServerCachableTuple(this ulong server)
    {
        return Tuple.Create<ulong, RestGuild?>(server, null);
    }

    public static DiscordCachableRole ToCachable(this RestRole role)
    {
        return new DiscordCachableRole(role.GuildId, role.Id);
    }

    public static Tuple<DiscordCachableRole, RestRole?> ToCachableTuple(this RestRole role)
    {
        return Tuple.Create<DiscordCachableRole, RestRole?>(role.ToCachable(), role);
    }

    public static Tuple<DiscordCachableRole, RestRole?> ToCachableTuple(this DiscordCachableRole role)
    {
        return Tuple.Create<DiscordCachableRole, RestRole?>(role, null);
    }
}
