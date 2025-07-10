using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class ModifyCategoryBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestCategoryChannel? Category = null;
        if (Block.inputs.TryGetValue("category", out WorkspaceBlockConnection? webhookBlock) && webhookBlock.block != null)
            Category = await Runtime.GetCategoryFromBlock(webhookBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify category, could not find category.");

        CategoryObjectBlock? CategoryObj = null;
        if (Block.inputs.TryGetValue("obj_category", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
            CategoryObj = DiscordBlocks.Parse(Runtime, webBlock.block) as CategoryObjectBlock;

        if (CategoryObj == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify category, category object data is missing.");

        string ChanName = await CategoryObj.Name();
        int? ChanPos = await CategoryObj.Position();
        await Category.ModifyAsync(x =>
        {
            if (!string.IsNullOrEmpty(ChanName))
                x.Name = ChanName;

            if (ChanPos.HasValue)
                x.Position = ChanPos.Value;
        });

        return null;
    }
}
