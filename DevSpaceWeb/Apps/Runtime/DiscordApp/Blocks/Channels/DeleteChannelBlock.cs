using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class DeleteChannelBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        IChannel? Channel = null;

        if (Block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
        {
            Channel = await Runtime.GetChannelFromBlock(chanBlock.block);

        }

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete channel, could not find channel.");

        if (Channel is IGuildChannel gc)
            await gc.DeleteAsync();
        else if (Channel is IDMChannel pc)
            await pc.CloseAsync();
        else if (Channel is IGroupChannel g)
            await g.LeaveAsync();
        else if (Channel is IForumChannel f)
            await f.DeleteAsync();
        else
            return new RuntimeError(RuntimeErrorType.Server, "Failed to delete channel, server could not run invalid action type.");


        return null;
    }
}
