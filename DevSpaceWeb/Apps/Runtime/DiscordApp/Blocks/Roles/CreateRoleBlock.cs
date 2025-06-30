using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Roles;

public class CreateRoleBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Server = null;

        if (Block.inputs.TryGetValue("server", out RequestBlocksBlock? roleBlock) && roleBlock.block != null)
            Server = await Runtime.GetServerFromBlock(roleBlock.block);

        if (Server == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create role, could not find server.");

        if (Server.Id == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageRoles)
            return Runtime.GetAppPermissionError(GuildPermission.ManageRoles);

        RoleObjectBlock? RoleObject = null;
        if (Block.inputs.TryGetValue("obj_role", out RequestBlocksBlock? webBlock) && webBlock.block != null)
            RoleObject = DiscordBlocks.Parse(Runtime, webBlock.block) as RoleObjectBlock;

        if (RoleObject == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create role, role object data is missing.");

        Color? RoleColor = await RoleObject.Color();
        RestRole CreatedRole = await Server.CreateRoleAsync(await RoleObject.Name(), RoleObject.Permissions(), RoleColor, await RoleObject.IsHoisted(), await RoleObject.IsMentionable());

        if (CreatedRole == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create role, could not find role.");

        if (Block.inputs.TryGetValue("output_role", out RequestBlocksBlock outputBlock) && outputBlock.block != null)
        {
            Runtime.Cache.AddRole(CreatedRole);
            Runtime.SetRoleData(outputBlock.block, CreatedRole.GuildId, CreatedRole);
        }

        return null;
    }
}
