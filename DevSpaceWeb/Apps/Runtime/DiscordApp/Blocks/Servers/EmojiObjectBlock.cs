using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;

public class EmojiObjectBlock : DiscordBlock
{
    public async Task<string?> Name()
    {
        if (Block.inputs.TryGetValue("name", out RequestBlocksBlock block) && block.block != null)
            return await Runtime.GetStringFromBlock(block.block);

        return null;
    }

    public async Task<List<RestRole>?> Roles()
    {
        if (Block.inputs.TryGetValue("obj_roles_list", out RequestBlocksBlock? rolesBlock) && rolesBlock.block != null)
        {
            List<RestRole> roles = new List<RestRole>();
            foreach (RequestBlocksBlock i in rolesBlock.block.inputs.Values)
            {
                if (i.block == null)
                    continue;

                RestRole? Role = await Runtime.GetRoleFromBlock(i.block);
                if (Role != null)
                    roles.Add(Role);
            }
            return roles;
        }
        return null;
    }
}
