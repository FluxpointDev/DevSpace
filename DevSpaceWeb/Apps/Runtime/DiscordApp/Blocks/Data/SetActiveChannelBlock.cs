using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveChannelBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestChannel? Channel = null;
        if (Block.inputs.TryGetValue("channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            Channel = await Runtime.GetChannelFromBlock(catBlock.block);

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active channel, could not find channel.");

        Runtime.Data.ChannelActive = Tuple.Create<ulong, RestChannel?>(Channel.Id, Channel);

        return null;
    }
}
