using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class DeleteMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out RequestBlocksBlock? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block);

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete message, could not find message.");

        await Message.DeleteAsync();

        return null;
    }
}
