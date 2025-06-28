using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActivePermissionBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        GuildPermissions? Permissions = null;
        if (Block.inputs.TryGetValue("permission", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            Permissions = Runtime.GetPermissionsFromBlock(catBlock.block);

        if (Permissions == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active server, could not find server.");

        Runtime.Data.PermissionsActive = Permissions;

        return null;
    }
}
