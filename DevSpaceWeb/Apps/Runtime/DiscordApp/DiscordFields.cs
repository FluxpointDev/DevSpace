using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordFields
{
    public static async Task Parse(DiscordRuntime runtime, WorkspaceBlock block)
    {
        if (block.inputs.TryGetValue("id", out WorkspaceBlockConnection? idBlock) && idBlock.block != null && idBlock.block.enabled)
        {
            switch (block.type)
            {
                case "obj_component_modal_input_text":
                    string Id = await runtime.GetStringFromBlock(idBlock.block);
                    RestMessageComponentData? Match = (runtime.Interaction as RestModal).Data.Components.Where(x => x.CustomId == Id).FirstOrDefault();
                    if (Match != null && block.inputs.TryGetValue("output_text", out WorkspaceBlockConnection? textBlock) && textBlock.block != null)
                        runtime.SetVariable(textBlock.block.GetVariableId(), Match.Value);

                    break;
            }

        }
        if (block.next != null && block.next.block != null)
            await Parse(runtime, block.next.block);
    }
}
