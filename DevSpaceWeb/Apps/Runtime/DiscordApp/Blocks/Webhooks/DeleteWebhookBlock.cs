using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Webhooks;

public class DeleteWebhookBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        Tuple<RestWebhook?, string>? Webhook = null;
        if (Block.inputs.TryGetValue("webhook", out WorkspaceBlockConnection? webhookBlock) && webhookBlock.block != null)
            Webhook = await Runtime.GetWebhookFromBlock(webhookBlock.block);

        if (Webhook == null || Webhook.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete webhook, could not find webhook.");

        await Webhook.Item1.DeleteAsync();
        return null;
    }
}
