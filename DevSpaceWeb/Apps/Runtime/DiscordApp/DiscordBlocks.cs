using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Roles;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Users;
using DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Webhooks;
using DevSpaceWeb.Apps.Runtime.Main;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordBlocks
{
    public static IBlock? Parse(DiscordRuntime runtime, RequestBlocks_Block data)
    {
        if (!data.enabled)
            return null;

        IBlock? Block = MainBlocks.Parse(runtime, data);
        if (Block == null)
        {
            try
            {
                switch (data.type)
                {
                    case "obj_text_channel":
                    case "obj_stage_channel":
                    case "obj_voice_channel":
                    case "obj_forum_channel":
                        Block = new ChannelObjectBlock();
                        break;
                    case "obj_category_channel":
                        Block = new CategoryObjectBlock();
                        break;
                    case "obj_message":
                        Block = new MessageObjectBlock();
                        break;
                    case "obj_embed":
                        Block = new EmbedObjectBlock();
                        break;
                    case "obj_role":
                        Block = new RoleObjectBlock();
                        break;
                    case "obj_webhook":
                        Block = new WebhookObjectBlock();
                        break;
                    case "obj_emoji":
                        Block = new EmojiObjectBlock();
                        break;
                    case "obj_components_list":
                        Block = new ComponentsObjectBlock();
                        break;
                }
            }
            catch { }

            if (Block != null && Block is DiscordBlock db)
                db.Set(runtime, runtime.Client, data);
        }


        return Block;
    }

    public static IActionBlock? ParseAction(DiscordRuntime runtime, RequestBlocks_Block data)
    {
        if (!data.enabled)
            return null;

        IActionBlock? Block = MainBlocks.ParseAction(runtime, data);

        if (Block == null)
        {
            try
            {
                switch (data.type)
                {
                    //
                    // Channels
                    //
                    case "action_delete_channel":
                        Block = new DeleteChannelBlock();
                        break;
                    case "action_create_category_channel":
                        Block = new CreateCategoryBlock();
                        break;
                    case "action_modify_category_channel":
                        Block = new ModifyCategoryBlock();
                        break;
                    case "action_delete_category":
                        Block = new DeleteCategoryBlock();
                        break;
                    case "action_create_text_channel":
                    case "action_create_voice_channel":
                    case "action_create_forum_channel":
                    case "action_create_announcement_channel":
                    case "action_create_stage_channel":
                        Block = new CreateChannelBlock();
                        break;
                    case "action_modify_text_channel":
                    case "action_modify_voice_channel":
                    case "action_modify_forum_channel":
                    case "action_modify_announcement_channel":
                    case "action_modify_stage_channel":
                        Block = new ModifyChannelBlock();
                        break;
                    //
                    // Members
                    //
                    case "action_add_role_member":
                        Block = new AddRoleBlock();
                        break;
                    case "action_set_nickname_member":
                        Block = new SetNicknameBlock();
                        break;
                    case "action_remove_timeout_member":
                        Block = new RemoveTimeoutBlock();
                        break;
                    case "action_set_timeout_member":
                        Block = new SetTimeoutBlock();
                        break;
                    case "action_remove_role_member":
                        Block = new RemoveRoleBlock();
                        break;
                    case "action_kick_member":
                        Block = new KickMemberBlock();
                        break;
                    case "action_ban_member":
                        Block = new BanMemberBlock();
                        break;
                    //
                    // Messages
                    //
                    case "action_send_response_message":
                    case "action_send_message":
                    case "action_send_webhook_message":
                    case "action_update_response_message":
                        Block = new SendMessageBlock();
                        break;
                    case "action_modify_message":
                        Block = new ModifyMessageBlock();
                        break;
                    case "action_delete_message":
                        Block = new DeleteMessageBlock();
                        break;
                    case "action_add_reaction_self":
                        Block = new AddReactionBlock();
                        break;
                    case "action_remove_reaction_self":
                    case "action_remove_reaction_member":
                    case "action_remove_all_reactions_member":
                    case "action_remove_reactions":
                    case "action_remove_all_reactions":
                        Block = new RemoveReactionBlock();
                        break;
                    case "action_pin_message":
                    case "action_unpin_message":
                        Block = new PinMessageBlock();
                        break;
                    case "action_crosspost_message":
                        Block = new CrossPostMessageBlock();
                        break;
                    //
                    // Roles
                    //
                    case "action_role_create":
                        Block = new CreateRoleBlock();
                        break;
                    case "action_role_modify":
                        Block = new ModifyRoleBlock();
                        break;
                    case "action_role_delete":
                        Block = new DeleteRoleBlock();
                        break;
                    //
                    // Servers
                    //
                    case "action_server_leave":
                        Block = new LeaveServerBlock();
                        break;
                    case "action_server_create_emoji":
                        Block = new CreateEmojiBlock();
                        break;
                    case "action_server_modify_emoji":
                        Block = new ModifyEmojiBlock();
                        break;
                    case "action_server_delete_emoji":
                        Block = new DeleteEmojiBlock();
                        break;
                    //
                    // Users
                    //
                    case "action_user_create_dm_channel":
                        Block = new CreatePrivateChannelBlock();
                        break;
                    //
                    // Webhooks
                    //
                    case "action_create_webhook":
                        Block = new CreateWebhookBlock();
                        break;
                    case "action_modify_webhook_message":
                        Block = new ModifyWebhookBlock();
                        break;
                    case "action_modify_webhook":
                        Block = new ModifyWebhookBlock();
                        break;
                    case "action_delete_webhook_message":
                        Block = new DeleteWebhookBlock();
                        break;
                    case "action_delete_webhook":
                        Block = new DeleteWebhookBlock();
                        break;
                    // Data
                    case "action_set_active_channel":
                        Block = new SetActiveChannelBlock();
                        break;
                    case "action_set_active_category":
                        Block = new SetActiveCategoryBlock();
                        break;
                    case "action_emoji_set_active":
                        Block = new SetActiveEmojiBlock();
                        break;
                    case "action_set_active_member":
                        Block = new SetActiveMemberBlock();
                        break;
                    case "action_set_active_message":
                        Block = new SetActiveMessageBlock();
                        break;
                    case "action_set_active_role":
                        Block = new SetActiveRoleBlock();
                        break;
                    case "action_set_active_server":
                        Block = new SetActiveServerBlock();
                        break;
                    case "action_set_active_user":
                        Block = new SetActiveUserBlock();
                        break;
                    case "action_set_active_webhook":
                        Block = new SetActiveWebhookBlock();
                        break;
                    case "action_set_active_permission":
                        Block = new SetActivePermissionBlock();
                        break;
                }
            }
            catch { }

            if (Block != null && Block is DiscordActionBlock db)
                db.Set(runtime, runtime.Client, data);
        }


        return Block;
    }
}
