using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;

public class LeaveServerBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Channel = null;

        if (Block.inputs.TryGetValue("server", out RequestBlocksBlock? chanBlock) && chanBlock.block != null)
            Channel = await Runtime.GetServerFromBlock(chanBlock.block);

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to leave server, could not find server.");

        await Channel.LeaveAsync();

        return null;
    }
}
