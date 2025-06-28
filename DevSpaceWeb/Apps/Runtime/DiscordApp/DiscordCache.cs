using Discord;
using Discord.Rest;
using Discord.Webhook;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public class DiscordCache
{
    public Dictionary<ulong, RestMessage> Messages = new Dictionary<ulong, RestMessage>();
    public void AddMessage(RestMessage message)
    {
        if (Messages.ContainsKey(message.Id))
            Messages[message.Id] = message;
        else
            Messages.Add(message.Id, message);
    }

    public Dictionary<ulong, RestChannel> Channels = new Dictionary<ulong, RestChannel>();
    public void AddChannel(RestChannel channel)
    {
        if (Channels.ContainsKey(channel.Id))
            Channels[channel.Id] = channel;
        else
            Channels.Add(channel.Id, channel);
    }

    public Dictionary<ulong, RestCategoryChannel> Category = new Dictionary<ulong, RestCategoryChannel>();
    public void AddCategory(RestCategoryChannel category)
    {
        if (Category.ContainsKey(category.Id))
            Category[category.Id] = category;
        else
            Category.Add(category.Id, category);
    }

    public Dictionary<ulong, Tuple<RestWebhook?, string>> Webhooks = new Dictionary<ulong, Tuple<RestWebhook?, string>>();
    public void AddWebhook(RestWebhook webhook, string token)
    {
        if (Webhooks.ContainsKey(webhook.Id))
            Webhooks[webhook.Id] = Tuple.Create<RestWebhook?, string>(webhook, token);
        else
            Webhooks.Add(webhook.Id, Tuple.Create<RestWebhook?, string>(webhook, token));
    }

    public Dictionary<ulong, DiscordWebhookClient> WebhookClients = new Dictionary<ulong, DiscordWebhookClient>();
    public void AddWebhookClient(RestWebhook data, DiscordWebhookClient webhook)
    {
        if (WebhookClients.ContainsKey(data.Id))
        {
            WebhookClients[data.Id].Dispose(); ;
            WebhookClients[data.Id] = webhook;
        }

        else
            WebhookClients.Add(data.Id, webhook);
    }

    public Dictionary<ulong, RestGuild> Servers = new Dictionary<ulong, RestGuild>();
    public void AddServer(RestGuild server)
    {
        if (Servers.ContainsKey(server.Id))
            Servers[server.Id] = server;
        else
            Servers.Add(server.Id, server);
    }

    public Dictionary<ulong, Tuple<RestGuild, GuildEmote>> Emojis = new Dictionary<ulong, Tuple<RestGuild, GuildEmote>>();
    public void AddEmoji(RestGuild server, GuildEmote emoji)
    {
        if (Emojis.ContainsKey(emoji.Id))
            Emojis[emoji.Id] = Tuple.Create(server, emoji);
        else
            Emojis.Add(emoji.Id, Tuple.Create(server, emoji));
    }

    public Dictionary<ulong, RestRole> Roles = new Dictionary<ulong, RestRole>();
    public void AddRole(RestRole role)
    {
        if (Roles.ContainsKey(role.Id))
            Roles[role.Id] = role;
        else
            Roles.Add(role.Id, role);
    }

    public Dictionary<string, RestGuildUser> Members = new Dictionary<string, RestGuildUser>();
    public void AddMember(RestGuildUser member)
    {
        if (Members.ContainsKey($"{member.GuildId}-{member.Id}"))
            Members[$"{member.GuildId}-{member.Id}"] = member;
        else
            Members.Add($"{member.GuildId}-{member.Id}", member);
    }

    public Dictionary<ulong, RestUser> Users = new Dictionary<ulong, RestUser>();
    public void AddUser(RestUser user)
    {
        if (Users.ContainsKey(user.Id))
            Users[user.Id] = user;
        else
            Users.Add(user.Id, user);
    }
}
