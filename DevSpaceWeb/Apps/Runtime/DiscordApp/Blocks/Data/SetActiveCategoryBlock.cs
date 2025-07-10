using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveCategoryBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestCategoryChannel? Category = null;
        if (Block.inputs.TryGetValue("category", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            Category = await Runtime.GetCategoryFromBlock(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active category, could not find category.");

        Runtime.Data.CategoryActive = Tuple.Create<ulong, RestCategoryChannel?>(Category.Id, Category);

        return null;
    }
}
