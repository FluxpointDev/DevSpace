using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Webhooks;

public class CreateWebhookBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        IChannel? Channel = null;

        if (Block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
        {
            Channel = await Runtime.GetChannelFromBlock(chanBlock.block);
        }

        if (Channel == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, could not find channel.");

        WebhookObjectBlock? Webhook = null;
        if (Block.inputs.TryGetValue("obj_webhook", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
            Webhook = DiscordBlocks.Parse(Runtime, webBlock.block) as WebhookObjectBlock;

        if (Webhook == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, webhook object data is missing.");

        WorkspaceBlock? HookBlock = null;
        if (Block.inputs.TryGetValue("output_webhook", out WorkspaceBlockConnection? hookBlock) && hookBlock.block != null)
            HookBlock = hookBlock.block;

        if (Channel is RestTextChannel tc)
        {
            RestWebhook? WH = null;
            if (Webhook.Avatar() != null)
            {
                if (Webhook.Avatar().Buffer == null)
                    return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, webhook avatar file data is missing.");

                using (MemoryStream stream = new MemoryStream(Webhook.Avatar().Buffer))
                    WH = await tc.CreateWebhookAsync(await Webhook.Name(),
                stream
                );
            }
            else
            {
                WH = await tc.CreateWebhookAsync(await Webhook.Name());
            }

            if (WH == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, could not find webhook.");

            if (HookBlock != null)
            {
                Runtime.Cache.AddWebhook(WH, "https://discord.com/api/webhooks/" + WH.Id + "/" + WH.Token);
                Runtime.SetWebhookData(HookBlock, Tuple.Create<RestWebhook?, string>(WH, "https://discord.com/api/webhooks/" + WH.Id + "/" + WH.Token));
            }
        }
        else if (Channel is RestForumChannel fc)
        {
            RestWebhook? WH = null;
            if (Webhook.Avatar() != null)
            {
                if (Webhook.Avatar().Buffer == null)
                    return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, webhook avatar file data is missing.");

                using (MemoryStream stream = new MemoryStream(Webhook.Avatar().Buffer))
                    WH = await fc.CreateWebhookAsync(await Webhook.Name(),
                stream
                );
            }
            else
            {
                WH = await fc.CreateWebhookAsync(await Webhook.Name());
            }

            if (WH == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create webhook, could not find webhook.");

            if (HookBlock != null)
            {
                Runtime.Cache.AddWebhook(WH, "https://discord.com/api/webhooks/" + WH.Id + "/" + WH.Token);
                Runtime.SetWebhookData(HookBlock, Tuple.Create<RestWebhook?, string>(WH, "https://discord.com/api/webhooks/" + WH.Id + "/" + WH.Token));
            }
        }
        else
            return new RuntimeError(RuntimeErrorType.Server, "Failed to create webhook, could not use invalid channel type.");
        

        return null;
    }
}
