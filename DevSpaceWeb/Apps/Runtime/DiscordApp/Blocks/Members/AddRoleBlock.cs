using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;

public class AddRoleBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? User = null;
        if (Block.inputs.TryGetValue("member", out WorkspaceBlockConnection? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetMemberFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add role to member, could not find member.");

        if (User.GuildId == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageRoles)
            return Runtime.GetAppPermissionError(GuildPermission.ManageRoles);

        string Reason = string.Empty;
        if (Block.inputs.TryGetValue("reason", out WorkspaceBlockConnection? rsBlock) && rsBlock.block != null)
            Reason = await Runtime.GetStringFromBlock(rsBlock.block);

        RestRole? Role = null;
        if (Block.inputs.TryGetValue("role", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
            Role = await Runtime.GetRoleFromBlock(roleBlock.block);

        if (Role == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add role to member, could not find role.");

        await User.AddRoleAsync(Role, !string.IsNullOrEmpty(Reason) ? new Discord.RequestOptions { AuditLogReason = Reason } : null);

        return null;
    }
}
