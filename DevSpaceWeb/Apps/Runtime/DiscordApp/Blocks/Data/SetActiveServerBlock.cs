using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveServerBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Server = null;
        if (Block.inputs.TryGetValue("server", out WorkspaceBlockConnection? srvBlock) && srvBlock.block != null)
            Server = await Runtime.GetServerFromBlock(srvBlock.block);

        if (Server == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active server, could not find server.");

        Runtime.Data.ServerActive = Tuple.Create<ulong, RestGuild?>(Server.Id, Server);

        return null;
    }
}