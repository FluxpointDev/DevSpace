using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class CategoryObjectBlock : DiscordBlock
{
    public async Task<string> Name()
    {
        if (Block.inputs.TryGetValue("name", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetStringFromBlock(block.block);

        return null;
    }

    public async Task<int?> Position()
    {
        if (Block.inputs.TryGetValue("position", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetIntFromBlock(block.block);

        return null;
    }

    public GuildPermissions? Permissions()
    {
        if (Block.inputs.TryGetValue("permissions", out WorkspaceBlockConnection block) && block.block != null)
            return Runtime.GetPermissionsFromBlock(block.block);

        return null;
    }
}
