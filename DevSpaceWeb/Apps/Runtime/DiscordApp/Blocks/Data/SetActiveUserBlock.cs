using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveUserBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestUser? User = null;
        if (Block.inputs.TryGetValue("user", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            User = await Runtime.GetUserFromBlock(catBlock.block);

        if (User == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active user, could not find user.");

        Runtime.Data.UserActive = Tuple.Create<ulong, RestUser?>(User.Id, User);

        return null;
    }
}
