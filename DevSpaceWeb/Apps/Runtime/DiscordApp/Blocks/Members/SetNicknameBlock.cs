using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;

public class SetNicknameBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? User = null;
        if (Block.inputs.TryGetValue("member", out WorkspaceBlockConnection? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetMemberFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set nickname for member, could not find member.");

        if (User.Id != Runtime.Interaction.ApplicationId && User.GuildId == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageNicknames)
            return Runtime.GetAppPermissionError(GuildPermission.ManageNicknames);

        string Nickname = string.Empty;
        if (Block.inputs.TryGetValue("name", out WorkspaceBlockConnection? nickBlock) && nickBlock.block != null)
            Nickname = await Runtime.GetStringFromBlock(nickBlock.block);

        string Reason = string.Empty;
        if (Block.inputs.TryGetValue("reason", out WorkspaceBlockConnection? rsBlock) && rsBlock.block != null)
            Reason = await Runtime.GetStringFromBlock(rsBlock.block);

        await User.ModifyAsync(x => x.Nickname = Nickname, !string.IsNullOrEmpty(Reason) ? new Discord.RequestOptions { AuditLogReason = Reason } : null);

        return null;
    }
}
