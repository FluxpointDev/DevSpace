using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Category = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            Category = await Runtime.GetMessageFromBlock(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active message, could not find message.");

        Runtime.Data.MessageActive = Tuple.Create<DiscordCachableMessage, RestMessage?>(new DiscordCachableMessage(Category.Channel.Id, Category.Id), Category);

        return null;
    }
}