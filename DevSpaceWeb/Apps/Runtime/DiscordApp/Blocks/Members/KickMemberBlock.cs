using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;

public class KickMemberBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? User = null;
        if (Block.inputs.TryGetValue("member", out RequestBlocksBlock? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetMemberFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to kick member, could not find member.");

        if (User.GuildId == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.KickMembers)
            return Runtime.GetAppPermissionError(GuildPermission.KickMembers);

        string Reason = string.Empty;
        if (Block.inputs.TryGetValue("reason", out RequestBlocksBlock? rsBlock) && rsBlock.block != null)
            Reason = await Runtime.GetStringFromBlock(rsBlock.block);

        await User.KickAsync(Reason);

        return null;
    }
}
