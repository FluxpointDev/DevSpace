using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.Webhook;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class SendMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        string? Content = null;
        EmbedBuilder? Embed = null;
        FileData? File = null;
        bool IsSpoiler = false;


        RestChannel? Channel = null;
        Tuple<RestWebhook?, string>? Webhook = null;

        if (Block.inputs.TryGetValue("webhook", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
        {
            Webhook = await Runtime.GetWebhookFromBlock(webBlock.block);
            if (Webhook == null || Webhook.Item1 == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send webhook message, could not find webhook.");

        }


        if (Block.inputs.TryGetValue("channel", out WorkspaceBlockConnection? channelBlock) && channelBlock.block != null)
            Channel = await Runtime.GetChannelFromBlock(channelBlock.block);

        MessageComponent? components = null;

        if (Channel == null && Block.type != "action_stop_execution" && Block.type != "action_send_response_message" && Block.type != "action_send_webhook_message" && Block.type != "action_update_response_message")
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, could not find channel.");

        if (Channel is RestForumChannel)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, you can't send normal messages to forum channels.");

        if (Block.inputs.TryGetValue("obj_message", out WorkspaceBlockConnection? MessageData) && MessageData.block != null)
        {
            MessageObjectBlock? messageBlock = DiscordBlocks.Parse(Runtime, MessageData.block) as MessageObjectBlock;
            if (messageBlock != null)
            {
                Content = await messageBlock.Content();
                File = messageBlock.File();

                if (messageBlock.Block.inputs.TryGetValue("is_file_spoiler", out WorkspaceBlockConnection? spoilerBlock) && spoilerBlock.block != null)
                    IsSpoiler = (await Runtime.GetBoolFromBlock(spoilerBlock.block)).GetValueOrDefault(false);

                EmbedObjectBlock? EmbedObject = messageBlock.Embed();

                if (EmbedObject != null)
                    Embed = await EmbedObject.Embed();

                if (messageBlock.Block.inputs.TryGetValue("obj_components_list", out WorkspaceBlockConnection? cmpBlock) && cmpBlock.block != null)
                {
                    ComponentsObjectBlock? componentsBlock = DiscordBlocks.Parse(Runtime, cmpBlock.block) as ComponentsObjectBlock;
                    if (componentsBlock != null)
                    {
                        ComponentBuilder? comp = await componentsBlock.GetComponents();
                        if (comp != null && comp.ActionRows.Count != 0 && comp.ActionRows.First().Components.Count != 0)
                            components = comp.Build();
                    }
                }
            }
        }





        if (string.IsNullOrEmpty(Content) && Embed == null && File == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, message does not have any content, embed or file to send");

        RestMessage? OutputMessage = null;
        ulong OutputMessageID = 0;
        if (Channel == null && Webhook == null)
        {
            if (Block.type == "action_update_response_message")
            {
                if (Runtime.InteractionResponse == null)
                    return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify message, no previous interaction message has been sent.");

                if (File != null)
                {
                    using (MemoryStream stream = new MemoryStream(File.Buffer))
                    {
                        await Runtime.InteractionResponse.ModifyAsync(x =>
                        {
                            x.Content = Content != null ? Optional.Create(Content) : Optional<string>.Unspecified;
                            x.Components = components != null ? Optional.Create(components) : Optional<MessageComponent>.Unspecified;
                            x.Embed = Embed != null ? Embed.Build() : Optional<Embed>.Unspecified;
                            x.Attachments = File.Buffer != null ? new FileAttachment[] { new FileAttachment(stream, (IsSpoiler ? "spoiler_" : "") + File.Name, null, IsSpoiler) } : Optional.Create<IEnumerable<FileAttachment>>(Array.Empty<FileAttachment>());
                        });
                    }
                }
                else
                {
                    await Runtime.InteractionResponse.ModifyAsync(x =>
                    {
                        x.Content = Content != null ? Optional.Create(Content) : Optional<string>.Unspecified;
                        x.Components = components != null ? Optional.Create(components) : Optional<MessageComponent>.Unspecified;
                        x.Embed = Embed != null ? Embed.Build() : Optional<Embed>.Unspecified;
                    });
                }
            }
            else
            {
                if (File != null)
                {
                    if (File.Buffer == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, file data is missing.");

                    using (MemoryStream stream = new MemoryStream(File.Buffer))
                        Runtime.InteractionResponse = await Runtime.Interaction.FollowupWithFileAsync(stream, (IsSpoiler ? "spoiler_" : "") + File.GetFileName(),
                                    Content,
                                    Embed != null ? new Embed[] { Embed.Build() } : null, false,
                                    Runtime.IsEphemeral(),
                                    Runtime.AllowedMentions,
                                    components: components
                                    );
                    OutputMessage = Runtime.InteractionResponse;
                }
                else
                {
                    Runtime.InteractionResponse = await Runtime.Interaction.FollowupAsync(
                                Content,
                                Embed != null ? new Embed[] { Embed.Build() } : null, false,
                                Runtime.IsEphemeral(),
                                Runtime.AllowedMentions,
                                components: components
                                );
                    OutputMessage = Runtime.InteractionResponse;

                    if (Runtime.IsEphemeral())
                    {
                        bool IsEphCheck = false;
                        if (Runtime.InteractionResponse.Flags.HasValue)
                            IsEphCheck = Runtime.InteractionResponse.Flags.Value.HasFlag(MessageFlags.Ephemeral);

                        if (!IsEphCheck)
                        {
                            _ = Runtime.InteractionResponse.DeleteAsync();
                            return new RuntimeError(RuntimeErrorType.Server, "Failed to send message, ephemeral response error.");
                        }
                    }

                }
            }

        }
        else if (Webhook != null)
        {
            string? WebhookName = null;
            string? WebhookAvatar = null;
            if (Block.inputs.TryGetValue("obj_webhook_message", out WorkspaceBlockConnection? whMsgBlock) && whMsgBlock.block != null)
            {
                if (whMsgBlock.block.inputs.TryGetValue("name", out WorkspaceBlockConnection? nameBlock) && nameBlock.block != null)
                    WebhookName = await Runtime.GetStringFromBlock(nameBlock.block);

                if (string.IsNullOrEmpty(WebhookName))
                    WebhookName = null;

                if (whMsgBlock.block.inputs.TryGetValue("avatar_url", out nameBlock) && nameBlock.block != null)
                    WebhookAvatar = await Runtime.GetStringFromBlock(nameBlock.block);

                if (string.IsNullOrEmpty(WebhookAvatar))
                    WebhookAvatar = null;
            }

            DiscordWebhookClient Client = Runtime.GetWebhookClient(Webhook);
            if (File != null)
            {
                if (File.Buffer == null)
                    return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, file data is missing.");

                using (MemoryStream stream = new MemoryStream(File.Buffer))
                    OutputMessage = await Client.SendFileWaitAsync(stream: stream, File.GetFileName(), Content, false, Embed != null ? new Embed[] { Embed.Build() } : null, WebhookName, WebhookAvatar, isSpoiler: IsSpoiler, allowedMentions: Runtime.AllowedMentions,
                                components: components);
            }
            else
            {
                OutputMessageID = await Client.SendMessageAsync(Content, false, Embed != null ? new Embed[] { Embed.Build() } : null, WebhookName, WebhookAvatar, allowedMentions: Runtime.AllowedMentions,
                                components: components);
            }
        }
        else
        {
            try
            {
                if (File != null)
                {
                    if (File.Buffer == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, file data is missing.");

                    using (MemoryStream stream = new MemoryStream(File.Buffer))
                        OutputMessage = await (Channel as IRestMessageChannel).SendFileAsync(stream: stream, File.GetFileName(),
                                    Content,
                                    false,
                                    Embed != null ? Embed.Build() : null,
                                    allowedMentions: Runtime.AllowedMentions,
                                    isSpoiler: IsSpoiler,
                                    components: components
                                    ) as RestMessage;
                }
                else
                {
                    OutputMessage = await (Channel as IRestMessageChannel).SendMessageAsync(
                                Content,
                                false,
                                Embed != null ? Embed.Build() : null,
                                allowedMentions: Runtime.AllowedMentions,
                                    components: components
                                ) as RestMessage;
                }
            }
            catch (HttpException he) when (he.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
            {
                return new RuntimeError(RuntimeErrorType.Runtime, $"This app can't send messages to **{(Channel as RestDMChannel).Recipient.Username}** because they have limited or blocked private messages.");
            }
        }

        if (OutputMessage == null && OutputMessageID == 0)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send message, could not find message.");

        if (Block.inputs.TryGetValue("output_message", out WorkspaceBlockConnection outputBlock) && outputBlock.block != null)
        {
            if (OutputMessage != null)
            {
                Runtime.Cache.AddMessage(OutputMessage);
                Runtime.SetMessageData(outputBlock.block, OutputMessage);
            }
            else if (OutputMessageID != 0)
            {
                Runtime.SetMessageData(outputBlock.block, Channel?.Id ?? 0, OutputMessageID);
            }
        }

        return null;
    }
}
