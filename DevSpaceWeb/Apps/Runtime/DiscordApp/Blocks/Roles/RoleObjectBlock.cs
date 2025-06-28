using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Roles;

public class RoleObjectBlock : DiscordBlock
{
    public async Task<string> Name()
    {
        if (Block.inputs.TryGetValue("name", out RequestBlocksBlock block) && block.block != null)
            return await Runtime.GetStringFromBlock(block.block);

        return null;
    }

    public Color? Color()
    {
        if (Block.inputs.TryGetValue("color", out RequestBlocksBlock block) && block.block != null)
            return Runtime.GetColorFromBlock(block.block);

        return null;
    }

    public GuildPermissions? Permissions()
    {
        if (Block.inputs.TryGetValue("permissions", out RequestBlocksBlock block) && block.block != null)
            return Runtime.GetPermissionsFromBlock(block.block);

        return null;
    }

    
    public async Task<bool> IsHoisted()
    {
        if (Block.inputs.TryGetValue("is_hoisted", out RequestBlocksBlock block) && block.block != null)
            return await Runtime.GetBoolFromBlock(block.block) ?? false;

        return false;
    }

    public async Task<bool> IsMentionable()
    {
        if (Block.inputs.TryGetValue("is_mentionable", out RequestBlocksBlock block) && block.block != null)
            return await Runtime.GetBoolFromBlock(block.block) ?? false;

        return false;
    }
}
