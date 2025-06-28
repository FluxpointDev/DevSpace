namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public interface IDiscordCachable
{

}

public class DiscordCachableMessage : IDiscordCachable
{
    public DiscordCachableMessage(ulong chan, ulong msg)
    {
        ChannelId = chan;
        MessageId = msg;
    }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
}
public class DiscordCachableServer : IDiscordCachable
{
    public DiscordCachableServer(ulong server)
    {
        ServerId = server;
    }

    public ulong ServerId { get; set; }
}
public class DiscordCachableRole : IDiscordCachable
{
    public DiscordCachableRole(ulong server, ulong role)
    {
        serverId = server;
        roleId = role;
    }
    public ulong serverId { get; set; }
    public ulong roleId { get; set; }
}
public class DiscordCachableMember : IDiscordCachable
{
    public DiscordCachableMember(ulong server, ulong member)
    {
        serverId = server;
        memberId = member;
    }
    public ulong serverId { get; set; }
    public ulong memberId { get; set; }
}
public class DiscordCachableUser : IDiscordCachable
{
    public DiscordCachableUser(ulong user)
    {
        UserId = user;
    }

    public ulong UserId { get; set; }
}
public class DiscordCachableChannel : IDiscordCachable
{
    public DiscordCachableChannel(ulong channel)
    {
        ChannelId = channel;
    }

    public ulong ChannelId { get; set; }
}
public class DiscordCachableCategory : IDiscordCachable
{
    public DiscordCachableCategory(ulong channel)
    {
        ChannelId = channel;
    }

    public ulong ChannelId { get; set; }
}
public class DiscordCachableEmoji : IDiscordCachable
{
    public DiscordCachableEmoji(ulong server, string emoji)
    {
        ServerId = server;
        Emoji = emoji;
    }

    public ulong ServerId { get; set; }
    public string Emoji { get; set; }
}