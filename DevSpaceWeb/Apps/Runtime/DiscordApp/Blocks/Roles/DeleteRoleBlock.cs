using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Roles;

public class DeleteRoleBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestRole? Role = null;

        if (Block.inputs.TryGetValue("role", out WorkspaceBlockConnection? roleBlock) && roleBlock.block != null)
            Role = await Runtime.GetRoleFromBlock(roleBlock.block);

        if (Role == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete role, could not find role.");

        await Role.DeleteAsync();
        

        return null;
    }
}
