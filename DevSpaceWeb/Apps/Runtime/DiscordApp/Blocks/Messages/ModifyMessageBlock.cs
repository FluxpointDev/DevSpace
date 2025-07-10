using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class ModifyMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block);

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify message, could not find message.");

        MessageObjectBlock? MessageObject = null;
        MessageComponent? components = null;

        if (Block.inputs.TryGetValue("obj_message", out WorkspaceBlockConnection? webBlock) && webBlock.block != null)
        {
            MessageObject = DiscordBlocks.Parse(Runtime, webBlock.block) as MessageObjectBlock;

            if (messageBlock.block.inputs.TryGetValue("obj_components_list", out WorkspaceBlockConnection? cmpBlock) && cmpBlock.block != null)
            {
                ComponentsObjectBlock? componentsBlock = DiscordBlocks.Parse(Runtime, cmpBlock.block) as ComponentsObjectBlock;
                if (componentsBlock != null)
                {
                    ComponentBuilder? comp = await componentsBlock.GetComponents();
                    if (comp != null && comp.ActionRows.Count != 0 && comp.ActionRows.First().Components.Count != 0)
                        components = comp.Build();
                }
                else
                    components = new ComponentBuilder() { ActionRows = new List<ActionRowBuilder> { } }.Build();
            }
        }
            

        if (MessageObject == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify message, message object data is missing.");

        string Content = await MessageObject.Content();

        

        if (Message is RestUserMessage rm)
            await rm.ModifyAsync(x =>
            {
                if (MessageObject.Block.inputs.ContainsKey("content"))
                    x.Content = Content;

                if (MessageObject.Block.inputs.ContainsKey("obj_components_list"))
                    x.Components = components;
            });

        return null;
    }
}
