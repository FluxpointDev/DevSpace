using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class DeleteCategoryBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestCategoryChannel? Category = null;
        if (Block.inputs.TryGetValue("category", out WorkspaceBlockConnection? webhookBlock) && webhookBlock.block != null)
            Category = await Runtime.GetCategoryFromBlock(webhookBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete category, could not find category.");

        await Category.DeleteAsync();

        return null;
    }
}