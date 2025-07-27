using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class ComponentsObjectBlock : DiscordBlock
{
    public Task<ComponentBuilder?> GetComponents() => GetAllComponents(Runtime, Block);

    public static async Task<ComponentBuilder?> GetAllComponents(DiscordRuntime Runtime, WorkspaceBlock block)
    {
        ComponentBuilder builder = new ComponentBuilder();
        foreach (WorkspaceBlockConnection row in block.inputs.Values)
        {
            if (row.block == null || row.block.type != "obj_component_row")
                continue;

            ActionRowBuilder actionRowBuilder = new ActionRowBuilder();
            foreach (WorkspaceBlockConnection comp in row.block.inputs.Values)
            {
                if (comp.block == null)
                    continue;

                IMessageComponent? component = await ParseComponent(Runtime, comp);
                if (component == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to parse components, invalid component block");

                if (actionRowBuilder.Components.Count == 5)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to parse components, component row count is 5 maximum");
                actionRowBuilder.AddComponent(component);
            }

            if (actionRowBuilder.Components.Count != 0)
                builder.AddRow(actionRowBuilder);
        }

        if (builder.ActionRows == null)
            builder.ActionRows = new List<ActionRowBuilder>();

        return builder;
    }

    public static async Task<IMessageComponent?> ParseComponent(DiscordRuntime Runtime, WorkspaceBlockConnection connection)
    {
        IMessageComponent? component = null;
        if (connection == null || connection.block == null)
            return null;

        switch (connection.block.type)
        {
            case "obj_component_button_link":
                {
                    string Label = string.Empty;
                    if (connection.TryGetInput("label", out WorkspaceBlock? lbBlock) && lbBlock != null)
                        Label = await Runtime.GetStringFromBlock(lbBlock);

                    string Url = string.Empty;
                    if (connection.TryGetInput("url", out lbBlock) && lbBlock != null)
                        Url = await Runtime.GetStringFromBlock(lbBlock);

                    Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emoji = null;
                    if (connection.TryGetInput("emoji", out lbBlock) && lbBlock != null)
                        Emoji = await Runtime.GetEmojiFromBlockAsync(lbBlock);

                    bool IsDisabled = false;
                    if (connection.TryGetInput("is_disabled", out lbBlock) && lbBlock != null)
                        IsDisabled = (await Runtime.GetBoolFromBlock(lbBlock)).GetValueOrDefault(false);

                    component = new ButtonBuilder(Label, "ln" + Label.ToLower().Replace(" ", "_"), ButtonStyle.Link, Url, Emoji?.Item2, IsDisabled).Build();
                }
                break;
            case "obj_component_button":
                {
                    string Label = string.Empty;
                    if (connection.TryGetInput("label", out WorkspaceBlock? lbBlock) && lbBlock != null)
                        Label = await Runtime.GetStringFromBlock(lbBlock);

                    string Id = string.Empty;
                    if (connection.TryGetInput("id", out lbBlock) && lbBlock != null)
                        Id = await Runtime.GetStringFromBlock(lbBlock);

                    Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emoji = null;
                    if (connection.TryGetInput("emoji", out lbBlock) && lbBlock != null)
                        Emoji = await Runtime.GetEmojiFromBlockAsync(lbBlock);

                    if (connection.TryGetInput("data", out lbBlock) && lbBlock != null)
                    {
                        string[] Split = lbBlock.type.Split('_');
                        switch ($"{Split[0]}_{Split[1]}")
                        {
                            case "data_message":
                                {
                                    RestMessage? message = await Runtime.GetMessageFromBlock(lbBlock);
                                    if (message == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get message for interaction button data in message components..");

                                    Id += $"|ms{message.Channel.Id}-{message.Id}";
                                }
                                break;
                            case "data_channel":
                                {
                                    RestChannel? channel = await Runtime.GetChannelFromBlock(lbBlock);
                                    if (channel == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get channel for interaction button data in message components.");

                                    Id += $"|ch{channel.Id}";
                                }
                                break;
                            case "data_category":
                                {
                                    RestCategoryChannel? channel = await Runtime.GetCategoryFromBlock(lbBlock);
                                    if (channel == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get category for interaction button data in message components.");

                                    Id += $"|ct{channel.Id}";
                                }
                                break;
                            case "data_server":
                                {
                                    RestGuild? server = await Runtime.GetServerFromBlock(lbBlock);
                                    if (server == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get server for interaction button data in message components.");

                                    Id += $"|sv{server.Id}";
                                }
                                break;
                            case "data_role":
                                {
                                    RestRole? role = await Runtime.GetRoleFromBlock(lbBlock);
                                    if (role == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get role for interaction button data in message components.");

                                    if (role.GuildId == Runtime.Data.ServerCurrent?.Item1)
                                        Id += $"|rl{role.Id}";
                                    else
                                        Id += $"|rl{role.GuildId}-{role.Id}";
                                }
                                break;
                            case "data_member":
                                {
                                    RestGuildUser? member = await Runtime.GetMemberFromBlock(lbBlock);
                                    if (member == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get member for interaction button data in message components.");

                                    if (member.GuildId == Runtime.Data.ServerCurrent?.Item1)
                                        Id += $"|rl{member.Id}";
                                    else
                                        Id += $"|rl{member.GuildId}-{member.Id}";
                                }
                                break;
                            case "data_user":
                                {
                                    RestUser? user = await Runtime.GetUserFromBlock(lbBlock);
                                    if (user == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get user for interaction button data in message components.");

                                    Id += $"|ur{user.Id}";
                                }
                                break;

                        }
                    }

                    bool IsDisabled = false;
                    if (connection.TryGetInput("is_disabled", out lbBlock) && lbBlock != null)
                        IsDisabled = (await Runtime.GetBoolFromBlock(lbBlock)).GetValueOrDefault(false);

                    ButtonStyle Style = ButtonStyle.Primary;

                    switch (connection.block.fields["style_type"].ToString())
                    {
                        case "secondary":
                            Style = ButtonStyle.Secondary;
                            break;
                        case "success":
                            Style = ButtonStyle.Success;
                            break;
                        case "danger":
                            Style = ButtonStyle.Danger;
                            break;
                    }
                    component = new ButtonBuilder(Label, "bt" + Id, Style, null, Emoji?.Item2, IsDisabled).Build();
                }
                break;
            case "obj_component_select":
                break;
        }
        return component;
    }
}
