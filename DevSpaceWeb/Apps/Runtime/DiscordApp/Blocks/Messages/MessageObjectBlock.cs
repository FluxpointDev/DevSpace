namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class MessageObjectBlock : DiscordBlock
{
    public async Task<string> Content()
    {
        if (Block.inputs.TryGetValue("content", out RequestBlocksBlock? content) && content.block != null)
            return await Runtime.GetStringFromBlock(content.block);
        return string.Empty;
    }

    public EmbedObjectBlock? Embed()
    {
        if (Block.inputs.TryGetValue("obj_embed", out RequestBlocksBlock? embed) && embed.block != null)
            return DiscordBlocks.Parse(Runtime, embed.block) as EmbedObjectBlock;

        return null;
    }

    public FileData? File()
    {
        if (Block.inputs.TryGetValue("file", out RequestBlocksBlock? content) && content.block != null)
            return Runtime.GetFileFromBlock(content.block);
        return null;
    }


}
