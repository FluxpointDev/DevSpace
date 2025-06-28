using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Members;

public class SetTimeoutBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? User = null;
        if (Block.inputs.TryGetValue("member", out RequestBlocksBlock? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetMemberFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set timeout for member, could not find member.");

        if (User.GuildId == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ModerateMembers)
            return Runtime.GetAppPermissionError(GuildPermission.ModerateMembers);

        string Reason = string.Empty;
        if (Block.inputs.TryGetValue("reason", out RequestBlocksBlock? rsBlock) && rsBlock.block != null)
            Reason = await Runtime.GetStringFromBlock(rsBlock.block);

        string Type = string.Empty;
        if (Block.inputs.TryGetValue("type", out RequestBlocksBlock? typeBlock) && typeBlock.block != null)
            Type = await Runtime.GetStringFromBlock(typeBlock.block);

        int? TimeoutTime = 0;
        if (Block.inputs.TryGetValue("number", out RequestBlocksBlock? daysBlock) && daysBlock.block != null)
            TimeoutTime = await Runtime.GetIntFromBlock(daysBlock.block);

        if (!TimeoutTime.HasValue)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set timeout for member, timeout time is missing.");

        if (TimeoutTime.Value < 1)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set timeout for member, timeout time can't be less than 1.");

        TimeSpan TS;
        switch (Type)
        {
            case "seconds":
                TS = new TimeSpan(0, 0, TimeoutTime.Value);
                break;
            case "minutes":
                TS = new TimeSpan(0, TimeoutTime.Value, 0);
                break;
            case "hours":
                TS = new TimeSpan(TimeoutTime.Value, 0, 0);
                break;
            case "days":
                TS = new TimeSpan(TimeoutTime.Value * 24, 0, 0);
                break;
            default:
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set timeout for member, timeout type is invalid");
        }

        if (TS.TotalSeconds > 2419200)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set timeout for member, timeout type can't be more than 28 days.");

        await User.SetTimeOutAsync(TS, !string.IsNullOrEmpty(Reason) ? new Discord.RequestOptions { AuditLogReason = Reason } : null);



        return null;
    }
}
