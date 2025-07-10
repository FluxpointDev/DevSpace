using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveEmojiBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Category = null;
        if (Block.inputs.TryGetValue("emoji", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            Category = await Runtime.GetEmojiFromBlockAsync(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active emoji, could not find emoji.");

        Runtime.Data.EmojiActive = Category;

        return null;
    }
}