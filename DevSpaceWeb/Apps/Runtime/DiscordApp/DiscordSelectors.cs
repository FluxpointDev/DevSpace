using DevSpaceWeb.Apps.Runtime.Main;
using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordSelectors
{
    public static async Task<object?> Parse(DiscordRuntime runtime, WorkspaceBlock block)
    {
        object? data = MainSelectors.Parse(runtime, block);
        if (data == null)
        {
            switch (block.type)
            {
                case "data_selector_channel":
                    {
                        if (block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
                        {
                            RestChannel? channel = await runtime.GetChannelFromBlock(chanBlock.block);
                            if (channel == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return channel.Id.ToString();
                                case "name":
                                    {
                                        if (channel is IRestMessageChannel rm)
                                            return rm.Name;
                                        else if (channel is RestForumChannel fc)
                                            return fc.Name;
                                        else if (channel is IDMChannel dc)
                                            return dc.Name;
                                        else if (channel is RestGroupChannel gc)
                                            return gc.Name;

                                    }
                                    break;
                                case "category_id":
                                    {
                                        if (channel is RestTextChannel gc && gc.CategoryId.HasValue)
                                            return gc.CategoryId.Value.ToString();
                                        else if (channel is RestForumChannel fc && fc.CategoryId.HasValue)
                                            return fc.CategoryId.Value.ToString();
                                    }
                                    break;
                                case "guild_id":
                                    {
                                        if (channel is RestGuildChannel gc)
                                            return gc.GuildId.ToString();
                                    }
                                    break;
                                case "position":
                                    {
                                        if (channel is RestGuildChannel gc)
                                            return gc.Position;
                                    }
                                    break;

                            }
                        }
                    }
                    break;
                case "data_selector_category":
                    {
                        if (block.inputs.TryGetValue("category", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
                        {
                            RestCategoryChannel? category = await runtime.GetCategoryFromBlock(catBlock.block);
                            if (category == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return category.Id.ToString();
                                case "name":
                                    return category.Name;
                                case "guild_id":
                                    return category.GuildId;
                                case "position":
                                    return category.Position;
                            }
                        }
                    }
                    break;
                case "data_selector_emoji":
                    {
                        if (block.inputs.TryGetValue("emoji", out WorkspaceBlockConnection? emojiBlock) && emojiBlock.block != null)
                        {
                            Tuple<Tuple<ulong, RestGuild?>?, IEmote>? emoji = await runtime.GetEmojiFromBlockAsync(emojiBlock.block);
                            if (emoji == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    if (emoji.Item2 is GuildEmote ge)
                                        return ge.Id.ToString();
                                    break;
                                case "name":
                                    return emoji.Item2.Name;
                                case "guild_id":
                                    if (emoji.Item2 is GuildEmote ge2)
                                        return ge2.Id.ToString();
                                    break;

                            }
                        }
                    }
                    break;
                case "data_selector_member":
                    {
                        if (block.inputs.TryGetValue("member", out WorkspaceBlockConnection? memberBlock) && memberBlock.block != null)
                        {
                            RestGuildUser? member = await runtime.GetMemberFromBlock(memberBlock.block);
                            if (member == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return member.Id.ToString();
                                case "nickname":
                                    return member.Nickname;
                                case "guild_id":
                                    return member.GuildId.ToString();

                            }
                        }
                    }
                    break;
                case "data_selector_message":
                    {
                        if (block.inputs.TryGetValue("message", out WorkspaceBlockConnection? mesBlock) && mesBlock.block != null)
                        {
                            RestMessage? message = await runtime.GetMessageFromBlock(mesBlock.block);
                            if (message == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return message.Id.ToString();
                                case "content":
                                    return message.Content;

                            }
                        }
                    }
                    break;
                case "data_selector_role":
                    {
                        if (block.inputs.TryGetValue("role", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
                        {
                            RestRole? role = await runtime.GetRoleFromBlock(roleBlock.block);
                            if (role == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return role.Id.ToString();
                                case "name":
                                    return role.Name;
                                case "guild_id":
                                    return role.GuildId.ToString();

                            }
                        }
                    }
                    break;
                case "data_selector_server":
                    {
                        if (block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
                        {
                            RestGuild? server = await runtime.GetServerFromBlock(srvBlock.block);
                            if (server == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return server.Id.ToString();
                                case "name":
                                    return server.Name;
                                case "icon_url":
                                    {
                                        if (!string.IsNullOrEmpty(server.IconId))
                                            return "https://cdn.discordapp.com/icons/" + server.Id + "/" + server.IconId + ".webp?size=320";
                                    }
                                    break;

                            }
                        }
                    }
                    break;
                case "data_selector_user":
                    {
                        if (block.inputs.TryGetValue("user", out WorkspaceBlockConnection? userBlock) && userBlock.block != null)
                        {
                            RestUser? user = await runtime.GetUserFromBlock(userBlock.block);
                            if (user == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return user.Id.ToString();
                                case "username":
                                    return user.Username;
                                case "display_name":
                                    return user.GlobalName;
                                case "avatar_url":
                                    return user.GetAvatarUrl(ImageFormat.WebP, 320);

                            }
                        }
                    }
                    break;
                case "data_selector_webhook":
                    {
                        if (block.inputs.TryGetValue("webhook", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
                        {
                            Tuple<RestWebhook?, string>? webhook = await runtime.GetWebhookFromBlock(webBlock.block);
                            if (webhook == null)
                                return null;

                            switch (block.fields["property"].ToString())
                            {
                                case "id":
                                    return webhook.Item1.Id.ToString();
                                case "name":
                                    return webhook.Item1.Name;

                            }
                        }
                    }
                    break;
            }
        }
        return data;
    }
}
