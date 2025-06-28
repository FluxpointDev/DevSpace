using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Webhooks;

public class ModifyWebhookBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        WebhookObjectBlock? WebhookObj = null;
        if (Block.inputs.TryGetValue("obj_webhook", out RequestBlocksBlock? webBlock) && webBlock.block != null)
            WebhookObj = DiscordBlocks.Parse(Runtime, webBlock.block) as WebhookObjectBlock;

        if (WebhookObj == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify webhook, webhook object data is missing.");

        Tuple<RestWebhook?, string>? Webhook = null;
        if (Block.inputs.TryGetValue("webhook", out RequestBlocksBlock? webhookBlock) && webhookBlock.block != null)
            Webhook = await Runtime.GetWebhookFromBlock(webhookBlock.block);

        if (Webhook == null || Webhook.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify webhook, could not find webhook.");

        string WebhookName = await WebhookObj.Name();

        await Webhook.Item1.ModifyAsync(x =>
            {
                if (!string.IsNullOrEmpty(WebhookName))
                    x.Name = WebhookName;

                if (WebhookObj.Block.inputs.TryGetValue("avatar", out RequestBlocksBlock? WHAvatar) && WHAvatar.block != null)
                {
                    FileData? FD = Runtime.GetFileFromBlock(WHAvatar.block);
                    if (FD != null)
                    {
                        if (FD.Buffer == null)
                            x.Image = Optional.Create<Image?>(null);
                        else
                            x.Image = new Image(new MemoryStream(FD.Buffer));
                    }
                }
            });


        return null;
    }
}
