using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class CrossPostMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestUserMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block) as RestUserMessage;

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to crosspost message, could not find message.");

        await Message.CrosspostAsync();

        return null;
    }
}
