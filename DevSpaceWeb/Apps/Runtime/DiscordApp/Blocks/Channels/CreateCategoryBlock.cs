using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class CreateCategoryBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Guild = null;
        if (Block.inputs.TryGetValue("server", out WorkspaceBlockConnection? servBlock) && servBlock.block != null)
            Guild = await Runtime.GetServerFromBlock(servBlock.block);

        if (Guild == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create category, could not find server.");

        if (Guild.Id == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ViewChannel)
            return Runtime.GetAppPermissionError(GuildPermission.ViewChannel);

        CategoryObjectBlock? category = null;
        if (Block.inputs.TryGetValue("obj_category_channel", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            category = DiscordBlocks.Parse(Runtime, catBlock.block) as CategoryObjectBlock;

        if (category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create category, category object data is missing.");

        WorkspaceBlock? CatBlock = null;
        if (Block.inputs.TryGetValue("output_category", out WorkspaceBlockConnection? hookBlock) && hookBlock.block != null)
            CatBlock = hookBlock.block;

        int? Pos = await category.Position();
        GuildPermissions? Perms = category.Permissions();

        if (Guild.Id == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageChannels)
            return Runtime.GetAppPermissionError(GuildPermission.ManageChannels);

        RestCategoryChannel Category = await Guild.CreateCategoryChannelAsync(await category.Name(), x =>
        {
            x.Position = !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);

        });

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create category, could not find category.");

        if (CatBlock != null)
        {
            Runtime.Cache.AddCategory(Category);
            Runtime.SetCategoryData(CatBlock, Category);
        }


        return null;
    }
}
