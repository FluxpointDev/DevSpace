using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks;

public static class DiscordOptions
{
    public static async Task<RuntimeError?> Parse(DiscordRuntime runtime, WorkspaceBlock block)
    {
        if (block.enabled)
        {
            if (runtime.Options == null)
                runtime.Options = new RuntimeOptions();

            switch (block.type)
            {
                case "option_allowmentions":
                    string MentionType = string.Empty;
                    if (block.fields.TryGetValue("mention_type", out Newtonsoft.Json.Linq.JToken? mentionField))
                        MentionType = mentionField.ToString();

                    if (!string.IsNullOrEmpty(MentionType))
                    {
                        switch (MentionType)
                        {
                            case "mention_reply":
                                if (runtime.AllowedMentions == null)
                                    return null;

                                runtime.AllowedMentions.MentionRepliedUser = true;
                                break;
                            case "mention_users":
                                if (runtime.AllowedMentions == null)
                                    return null;

                                if (runtime.AllowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.None))
                                    runtime.AllowedMentions.AllowedTypes = AllowedMentionTypes.Users;
                                else
                                    runtime.AllowedMentions.AllowedTypes = runtime.AllowedMentions.AllowedTypes.Value | AllowedMentionTypes.Users;
                                break;
                            case "mention_roles":
                                if (runtime.AllowedMentions == null)
                                    return null;

                                if (runtime.AllowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.None))
                                    runtime.AllowedMentions.AllowedTypes = AllowedMentionTypes.Roles;
                                else
                                    runtime.AllowedMentions.AllowedTypes = runtime.AllowedMentions.AllowedTypes.Value | AllowedMentionTypes.Roles;
                                break;
                            case "mention_everyone":
                                if (runtime.AllowedMentions == null)
                                    return null;

                                if (runtime.AllowedMentions.AllowedTypes.Value.HasFlag(AllowedMentionTypes.None))
                                    runtime.AllowedMentions.AllowedTypes = AllowedMentionTypes.Everyone;
                                else
                                    runtime.AllowedMentions.AllowedTypes = runtime.AllowedMentions.AllowedTypes.Value | AllowedMentionTypes.Everyone;
                                break;
                            case "mention_everything":
                                runtime.AllowedMentions = null;
                                break;
                        }
                    }
                    break;
                case "option_allowmentions_user":
                    {
                        if (runtime.AllowedMentions == null)
                            return null;

                        ulong? UserId = null;
                        if (block.inputs.TryGetValue("user", out WorkspaceBlockConnection? usrBlock) && usrBlock.block != null)
                            UserId = await runtime.GetUserIdFromBlock(usrBlock.block);

                        if (UserId.HasValue)
                            runtime.AllowedMentions.UserIds.Add(UserId.Value);
                    }
                    break;
                case "option_allowmentions_role":
                    {
                        if (runtime.AllowedMentions == null)
                            return null;

                        ulong? RoleId = null;
                        if (block.inputs.TryGetValue("role", out WorkspaceBlockConnection? usrBlock) && usrBlock.block != null)
                            RoleId = await runtime.GetRoleIdFromBlock(usrBlock.block);

                        if (RoleId.HasValue)
                            runtime.AllowedMentions.RoleIds.Add(RoleId.Value);
                    }
                    break;
                case "option_require_app_premium":
                    runtime.Options.RequireAppPremium = true;
                    break;
                case "option_nsfw_only":
                    runtime.Options.RequireNsfw = true;
                    break;
                case "option_app_owner_only":
                    runtime.Options.RequireAppOwner = true;
                    break;
                case "option_app_developer_only":
                    runtime.Options.RequireAppDeveloper = true;
                    break;
                case "option_server_owner_only":
                    runtime.Options.RequireServerOwner = true;
                    break;
                case "option_require_server":
                    runtime.Options.RequireServer = true;
                    break;
                case "option_require_private_channel":
                    runtime.Options.RequirePrivateChannel = true;
                    break;
                case "option_require_group_channel":
                    runtime.Options.RequireGroupChannel = true;
                    break;
                case "option_require_server_permission":
                    {
                        WorkspaceBlock? permBlock = null;
                        WorkspaceBlock? memberBlock = null;
                        if (block.inputs.TryGetValue("permission", out WorkspaceBlockConnection? pBlock) && pBlock.block != null)
                            permBlock = pBlock.block;

                        if (permBlock == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permission is missing permissions.");

                        if (block.inputs.TryGetValue("member", out WorkspaceBlockConnection? uBlock) && uBlock.block != null)
                            memberBlock = uBlock.block;

                        if (memberBlock == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permission is missing member.");

                        GuildPermissions? perms = runtime.GetPermissionsFromBlock(permBlock);
                        if (perms == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permissions has invalid permissions.");

                        runtime.Options.RequireServerPermissions.Add(Tuple.Create<GuildPermissions, WorkspaceBlock>(perms.Value, memberBlock));
                    }
                    break;
                case "option_require_channel_permission":
                    {
                        WorkspaceBlock? permBlock = null;
                        WorkspaceBlock? channelBlock = null;
                        if (block.inputs.TryGetValue("permission", out WorkspaceBlockConnection? pBlock) && pBlock.block != null)
                            permBlock = pBlock.block;

                        if (permBlock == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permission is missing permissions.");

                        if (block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? uBlock) && uBlock.block != null)
                            channelBlock = uBlock.block;

                        if (channelBlock == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permission is missing channel.");

                        if (block.inputs.TryGetValue("member", out WorkspaceBlockConnection? mBlock) && mBlock.block != null)
                            channelBlock.next = new WorkspaceBlockConnection { block = mBlock.block };

                        if (channelBlock.next == null || channelBlock.next.block == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permission is missing member.");


                        GuildPermissions? perms = runtime.GetPermissionsFromBlock(pBlock.block);
                        if (perms == null)
                            return new RuntimeError(RuntimeErrorType.Runtime, "Could not parse command options, require server permissions has invalid permissions.");

                        runtime.Options.RequireChannelPermissions.Add(Tuple.Create<ChannelPermissions, WorkspaceBlock>(new ChannelPermissions(perms.Value.RawValue), channelBlock));
                    }
                    break;
            }
        }

        if (block.next != null && block.next.block != null)
        {
            RuntimeError? error = await DiscordOptions.Parse(runtime, block.next.block);
            if (error != null)
                return error;
        }

        return null;
    }
}
public class RuntimeOptions
{
    public bool RequireAppPremium = false;
    public bool IsEphemeral = false;
    public bool RequireNsfw = false;
    public bool RequireAppOwner = false;
    public bool RequireAppDeveloper = false;
    public bool RequireServerOwner = false;
    public bool RequireServer = false;
    public bool RequirePrivateChannel = false;
    public bool RequireGroupChannel = false;

    public List<Tuple<GuildPermissions, WorkspaceBlock>> RequireServerPermissions = new List<Tuple<GuildPermissions, WorkspaceBlock>>();
    public List<Tuple<ChannelPermissions, WorkspaceBlock>> RequireChannelPermissions = new List<Tuple<ChannelPermissions, WorkspaceBlock>>();

}