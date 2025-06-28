using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;

public class BanMemberBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? User = null;
        if (Block.inputs.TryGetValue("member", out RequestBlocksBlock? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetMemberFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to ban member, could not find member.");

        if (User.GuildId == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.BanMembers)
            return Runtime.GetAppPermissionError(GuildPermission.BanMembers);

        string Reason = string.Empty;
        if (Block.inputs.TryGetValue("reason", out RequestBlocksBlock? rsBlock) && rsBlock.block != null)
            Reason = await Runtime.GetStringFromBlock(rsBlock.block);

        int? PruneDays = 0;
        if (Block.inputs.TryGetValue("prune", out RequestBlocksBlock? daysBlock) && daysBlock.block != null)
            PruneDays = await Runtime.GetIntFromBlock(daysBlock.block);

        if (PruneDays < 0)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to ban member, prune time can't be less than 0.");

        if (PruneDays > 7)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to ban member, prune time can't be more than 7 days");

        await User.BanAsync(PruneDays.HasValue ? PruneDays.Value : 0, Reason);


        return null;
    }
}
