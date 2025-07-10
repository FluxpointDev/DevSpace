using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class ChannelObjectBlock : DiscordBlock
{
    public async Task<string> Name()
    {
        if (Block.inputs.TryGetValue("name", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetStringFromBlock(block.block);

        return null;
    }

    public async Task<RestCategoryChannel?> Category()
    {
        if (Block.inputs.TryGetValue("category", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetCategoryFromBlock(block.block);

        return null;
    }

    public async Task<string> Topic()
    {
        if (Block.inputs.TryGetValue("topic", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetStringFromBlock(block.block);

        return null;
    }

    public async Task<int?> Slowmode()
    {
        if (Block.inputs.TryGetValue("slowmode", out WorkspaceBlockConnection block) && block.block != null)
            return await Runtime.GetIntFromBlock(block.block);

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

    public async Task<bool> IsNsfw()
    {
        if (Block.inputs.TryGetValue("is_nsfw", out WorkspaceBlockConnection block) && block.block != null)
            return (await Runtime.GetBoolFromBlock(block.block)).GetValueOrDefault(false);

        return false;
    }
}
