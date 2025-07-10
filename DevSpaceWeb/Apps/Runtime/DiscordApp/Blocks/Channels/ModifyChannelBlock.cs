using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class ModifyChannelBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestChannel? Channel = null;
        if (Block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
        {
            Channel = await Runtime.GetChannelFromBlock(chanBlock.block);
        }

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify channel, could not find channel.");

        ChannelObjectBlock? ChanObj = null;
        if (Block.inputs.TryGetValue("obj_channel", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
            ChanObj = DiscordBlocks.Parse(Runtime, webBlock.block) as ChannelObjectBlock;

        if (ChanObj == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify channel, channel object data is missing.");

        if (Channel is IGuildChannel g)
        {
            string ChanName = await ChanObj.Name();
            int? ChanPos = await ChanObj.Position();
            await g.ModifyAsync(async x =>
            {
                if (!string.IsNullOrEmpty(ChanName))
                    x.Name = ChanName;

                if (ChanPos.HasValue)
                    x.Position = ChanPos.Value;

                RestCategoryChannel? Cat = await ChanObj.Category();
                if (Cat != null)
                    x.CategoryId = Cat.Id;
            });
        }


        return null;
    }
}
