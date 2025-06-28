using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;

public class CreateEmojiBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Server = null;

        if (Block.inputs.TryGetValue("server", out RequestBlocksBlock? chanBlock) && chanBlock.block != null)
            Server = await Runtime.GetServerFromBlock(chanBlock.block);

        if (Server == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, could not find server.");

        EmojiObjectBlock? category = null;
        if (Block.inputs.TryGetValue("obj_emoji", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            category = DiscordBlocks.Parse(Runtime, catBlock.block) as EmojiObjectBlock;

        if (category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, emoji object data is missing.");

        if (string.IsNullOrEmpty(await category.Name()))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, emoji name is missing.");

        FileData? File = null;
        if (Block.inputs.TryGetValue("file", out RequestBlocksBlock? content) && content.block != null)
            File = Runtime.GetFileFromBlock(content.block);

        if (File == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, file data is missing.");

        if (File.Buffer == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, file data is empty.");

        List<RestRole>? Roles = await category.Roles();
        GuildEmote? Emoji = null;
        using (MemoryStream stream = new MemoryStream(File.Buffer))
            Emoji = await Server.CreateEmoteAsync(await category.Name(), new Image(stream), (Roles != null && Roles.Count != 0) ? Roles : Optional<IEnumerable<IRole>>.Unspecified);

        if (Emoji == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create emoji, could not find emoji.");

        if (Block.inputs.TryGetValue("output_emoji", out RequestBlocksBlock outputBlock) && outputBlock.block != null)
        {
            Runtime.Cache.AddEmoji(Server, Emoji);
            Runtime.SetEmojiData(outputBlock.block, Server, Emoji);
        }

        return null;
    }
}
