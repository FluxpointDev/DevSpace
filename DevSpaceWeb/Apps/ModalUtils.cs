using DevSpaceWeb.Apps.Runtime;
using DevSpaceWeb.Apps.Runtime.DiscordApp;
using Discord;

namespace DevSpaceWeb.Apps;

public static class ModalUtils
{
    public static async Task<Tuple<Modal?, RuntimeError?>> ParseDynamicModal(DiscordRuntime runtime, string modalId, WorkspaceBlock modalBlock)
    {
        ModalBuilder builder = new ModalBuilder().WithCustomId(modalId);
        if (!string.IsNullOrEmpty(runtime.ModalDataType))
            builder.CustomId += "|" + runtime.ModalDataType;

        try
        {
            if (modalBlock.inputs.TryGetValue("modal_title", out WorkspaceBlockConnection? titleBlock) && titleBlock.block != null)
            {
                string Test = await runtime.GetStringFromBlock(titleBlock.block);
                builder.WithTitle(Test);
            }

            if (modalBlock.inputs.TryGetValue("modal_fields", out WorkspaceBlockConnection? fieldBlock) && fieldBlock.block != null)
            {
                RuntimeError? error = await ModalUtils.ParseDynamicModalField(builder, modalId, runtime, fieldBlock.block);
                if (error != null)
                    return Tuple.Create<Modal?, RuntimeError?>(null, error);
            }
        }
        catch (RuntimeError re)
        {
            return Tuple.Create<Modal?, RuntimeError?>(null, new RuntimeError(re.Type, $"Modal error for {modalId}\n" + re.ErrorMessage));
        }
        catch (Exception ex)
        {
            return Tuple.Create<Modal?, RuntimeError?>(null, new RuntimeError(RuntimeErrorType.Server, $"Server failed to parse modal data for {modalId}"));
        }

        return Tuple.Create<Modal?, RuntimeError?>(builder.Build(), null);
    }

    public static async Task<RuntimeError?> ParseDynamicModalField(ModalBuilder builder, string modalId, DiscordRuntime runtime, WorkspaceBlock modalBlock)
    {
        string Id = string.Empty;
        if (modalBlock.inputs.TryGetValue("id", out WorkspaceBlockConnection? ip) && ip.block != null)
            Id = await runtime.GetStringFromBlock(ip.block);

        string Label = string.Empty;
        if (modalBlock.inputs.TryGetValue("label", out ip) && ip.block != null)
            Label = await runtime.GetStringFromBlock(ip.block);

        string Placeholder = string.Empty;
        if (modalBlock.inputs.TryGetValue("placeholder", out ip) && ip.block != null)
            Placeholder = await runtime.GetStringFromBlock(ip.block);

        string Value = string.Empty;
        if (modalBlock.inputs.TryGetValue("default_value", out ip) && ip.block != null)
            Value = await runtime.GetStringFromBlock(ip.block);

        bool Required = false;
        if (modalBlock.inputs.TryGetValue("is_required", out ip) && ip.block != null)
            Required = ip.block.fields["BOOL"].ToString() == "TRUE" ? true : false;

        TextInputStyle Style = TextInputStyle.Short;
        if (modalBlock.fields.TryGetValue("input_type", out Newtonsoft.Json.Linq.JToken? field))
            Style = field.ToString() == "short" ? TextInputStyle.Short : TextInputStyle.Paragraph;

        int? Min = null;
        if (modalBlock.inputs.TryGetValue("min_length", out ip) && ip.block != null)
            Min = (int)ip.block.fields["NUM"];

        int? Max = null;
        if (modalBlock.inputs.TryGetValue("max_length", out ip) && ip.block != null)
            Max = (int)ip.block.fields["NUM"];

        if (string.IsNullOrEmpty(Id))
        {
            return new RuntimeError(RuntimeErrorType.Runtime, $"Modal {modalId} has a missing field id.");
        }

        if (string.IsNullOrEmpty(Label))
        {
            return new RuntimeError(RuntimeErrorType.Runtime, $"Modal {modalId} has a missing field label.");
        }

        if (Min.HasValue && Min < 0)
        {
            return new RuntimeError(RuntimeErrorType.Runtime, $"Modal {modalId} has an invalid field min value less than 0.");
        }

        if (Max.HasValue && Max < 0)
        {
            if (Max < 0)
                return new RuntimeError(RuntimeErrorType.Runtime, $"Modal {modalId} has an invalid field max value less than 0.");

            if (Min.HasValue && Max.Value < Min.Value)
                return new RuntimeError(RuntimeErrorType.Runtime, $"Modal {modalId} has an invalid field max value is less than min value.");

        }

        builder.AddTextInput(new TextInputBuilder
        {
            CustomId = Id,
            Label = Label,
            Placeholder = Placeholder,
            Value = Value,
            Required = Required,
            Style = Style,
            MinLength = Min,
            MaxLength = Max
        });

        if (modalBlock.next != null && modalBlock.next.block != null)
        {
            RuntimeError? error = await ParseDynamicModalField(builder, modalId, runtime, modalBlock.next.block);
            if (error != null)
                return error;
        }

        return null;
    }

}
