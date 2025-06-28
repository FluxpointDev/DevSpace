using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveRoleBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestRole? Category = null;
        if (Block.inputs.TryGetValue("role", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            Category = await Runtime.GetRoleFromBlock(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active role, could not find role.");

        ulong? Server = null;
        if (catBlock.block.inputs.TryGetValue("server", out RequestBlocksBlock? srvBlock) && srvBlock.block != null)
            Server = await Runtime.GetServerIdFromBlock(srvBlock.block);

        if (Server == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to get server id from role.");

        Runtime.Data.RoleActive = Tuple.Create<DiscordCachableRole, RestRole?>(new DiscordCachableRole(Server.Value, Category.Id), Category);

        return null;
    }
}
