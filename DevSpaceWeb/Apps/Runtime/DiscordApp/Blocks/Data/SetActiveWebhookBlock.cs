using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveWebhookBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        Tuple<RestWebhook?, string>? Category = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            Category = await Runtime.GetWebhookFromBlock(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active webhook, could not find webhook.");

        Runtime.Data.WebhookActive = Category;

        return null;
    }
}
