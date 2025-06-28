using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Roles;

public class ModifyRoleBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestRole? Role = null;

        if (Block.inputs.TryGetValue("role", out RequestBlocksBlock? roleBlock) && roleBlock.block != null)
            Role = await Runtime.GetRoleFromBlock(roleBlock.block);

        if (Role == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify role, could not find role.");

        RoleObjectBlock? RoleObject = null;
        if (Block.inputs.TryGetValue("obj_role", out RequestBlocksBlock? webBlock) && webBlock.block != null)
            RoleObject = DiscordBlocks.Parse(Runtime, webBlock.block) as RoleObjectBlock;

        if (RoleObject == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify role, role object data is missing.");

        string RoleName = await RoleObject.Name();
        bool IsHoisted = await RoleObject.IsHoisted();
        bool IsMentionable = await RoleObject.IsMentionable();

        await Role.ModifyAsync(x =>
        {
            if (!string.IsNullOrEmpty(RoleName))
                x.Name = RoleName;

            if (RoleObject.Color().HasValue)
                x.Color = RoleObject.Color().Value;

            if (Block.inputs.ContainsKey("is_hoisted"))
                x.Hoist = IsHoisted;

            if (Block.inputs.ContainsKey("is_mentionable"))
                x.Mentionable = IsMentionable;

            if (RoleObject.Permissions().HasValue)
                x.Permissions = RoleObject.Permissions().Value;

        });


        return null;
    }
}
