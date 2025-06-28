using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Users;

public class CreatePrivateChannelBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestUser? User = null;
        if (Block.inputs.TryGetValue("user", out RequestBlocksBlock? webhookBlock) && webhookBlock.block != null)
            User = await Runtime.GetUserFromBlock(webhookBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create private user channel, could not find user.");

        RequestBlocks_Block? ChanBlock = null;
        if (Block.inputs.TryGetValue("output_channel", out RequestBlocksBlock? hookBlock) && hookBlock.block != null)
            ChanBlock = hookBlock.block;

        RestDMChannel Channel = await User.CreateDMChannelAsync();

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create private user channel, could not create channel.");

        if (ChanBlock != null)
        {
            Runtime.Cache.AddChannel(Channel);
            Runtime.SetChannelData(ChanBlock, Channel);
        }
        return null;
    }
}
