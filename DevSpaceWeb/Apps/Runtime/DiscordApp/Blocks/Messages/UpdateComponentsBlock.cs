using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class UpdateComponentsBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block);

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify components, could not find message.");

        if (!Message.Components.Any())
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify components, message does not have any components.");

        ComponentBuilder currentComponents = ComponentBuilder.FromMessage(Message);
        Dictionary<string, IMessageComponent> updatedComponents = new Dictionary<string, IMessageComponent>();
        if (Block.inputs.TryGetValue("obj_components_row", out WorkspaceBlockConnection? cmpBlock) && cmpBlock.block != null)
        {
            foreach (WorkspaceBlockConnection comp in cmpBlock.block.inputs.Values)
            {
                IMessageComponent? component = await ComponentsObjectBlock.ParseComponent(Runtime, comp);
                if (component == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to parse components, invalid component block");

                foreach (ActionRowBuilder r in currentComponents.ActionRows)
                {
                    foreach (IMessageComponent c in r.Components)
                    {
                        if (c.CustomId == component.CustomId)
                            updatedComponents.Add(component.CustomId, component);
                    }
                }
            }
        }

        if (!updatedComponents.Any())
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify components, no components have been set.");

        ComponentBuilder NewComponents = new ComponentBuilder();
        foreach (ActionRowBuilder i in currentComponents.ActionRows)
        {
            ActionRowBuilder row = new ActionRowBuilder();
            foreach (IMessageComponent c in i.Components)
            {
                if (updatedComponents.TryGetValue(c.CustomId, out IMessageComponent? gotComponent))
                    row.AddComponent(gotComponent);
                else
                    row.AddComponent(c);
            }
            NewComponents.AddRow(row);
        }

        if (Message is RestUserMessage rm)
            await rm.ModifyAsync(x =>
            {
                x.Components = NewComponents.Build();
            });

        return null;
    }
}
