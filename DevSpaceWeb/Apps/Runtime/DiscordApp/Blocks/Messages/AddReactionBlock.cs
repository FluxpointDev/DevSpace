using Discord;
using Discord.Net;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class AddReactionBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block);

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add reaction to message, could not find message.");

        IEmote? EmojiObject = null;
        if (Block.inputs.TryGetValue("emoji", out WorkspaceBlockConnection? emojiBlock) && emojiBlock.block != null)
        {
            Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emotes = await Runtime.GetEmojiFromBlockAsync(emojiBlock.block);
            if (Emotes == null || Emotes.Item2 == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add reaction to message, emoji could not be parsed.");

            EmojiObject = Emotes.Item2;
        }

        if (EmojiObject == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add reaction to message, emoji is missing.");

        try
        {
            await Message.AddReactionAsync(EmojiObject);
        }
        catch (HttpException http) when (http.DiscordCode == DiscordErrorCode.UnknownEmoji)
        {
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add reaction to message, unknown emoji or bot does not have access to the emoji.");
        }
        return null;
    }
}
