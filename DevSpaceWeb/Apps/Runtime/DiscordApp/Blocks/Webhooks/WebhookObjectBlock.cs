namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Webhooks;

public class WebhookObjectBlock : DiscordBlock
{
    public async Task<string> Name()
    {
        if (Block.inputs.TryGetValue("name", out RequestBlocksBlock? content) && content.block != null)
            return await Runtime.GetStringFromBlock(content.block);
        return string.Empty;
    }

    public FileData? Avatar()
    {
        if (Block.inputs.TryGetValue("avatar", out RequestBlocksBlock? content) && content.block != null)
            return Runtime.GetFileFromBlock(content.block);
        return null;
    }
}
