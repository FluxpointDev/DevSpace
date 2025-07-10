using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class PinMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestUserMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block) as RestUserMessage;


        if (Block.type == "action_pin_message")
        {
            if (Message == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to pin message, could not find message.");

            await Message.PinAsync();
        }
        else if (Block.type == "action_unpin_message")
        {
            if (Message == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to unpin message, could not find message.");

            await Message.UnpinAsync();
        }
        else
            return new RuntimeError(RuntimeErrorType.Server, "Failed to pin message, server could not run invalid action.");



        return null;
    }
}
