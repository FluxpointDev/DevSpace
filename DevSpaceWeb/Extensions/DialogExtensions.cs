using DevSpaceWeb.Components.Dialogs;
using Radzen;

namespace DevSpaceWeb;

public static class DialogExtensions
{
    public static async Task<bool> ShowConfirmAsync(this DialogService service, string title, string text, string buttonText, ButtonStyle color)
    {
        DialogOptions options = new DialogOptions() { };

        dynamic Dialog = await service.OpenAsync<ConfirmInfoDialog>(title, new Dictionary<string, object>()
        {
            { "ContentText", text },
            { "ButtonText", buttonText },
            { "Color", color }
        }, options);


        if (Dialog == null)
            return false;

        return (bool)Dialog;
    }

    public static async Task<bool> ShowDynamicFormAsync<Model>(this DialogService service, string title, Model model, Func<Model, Task<string?>> function)
    {
        DialogOptions options = new DialogOptions() { AutoFocusFirstElement = true };

        dynamic dialog = await service.OpenAsync<DynamicFormDialog<Model>>(title, new Dictionary<string, object>()
        {
            { "ModelData", model },
            { "OnSubmit", function },
            { "IsDialog", true }
        }, options);

        if (dialog is not bool)
            return false;
        return dialog;
    }

    public static async Task<bool> ShowInfoAsync(this DialogService service, string title, string text)
    {
        DialogOptions options = new DialogOptions() { };

        dynamic Dialog = await service.OpenAsync<ConfirmInfoDialog>(title, new Dictionary<string, object>()
        {
            { "ContentText", text },
            { "ButtonText", "Ok" },
            { "Color", ButtonStyle.Success },
            { "ShowCancel", false }
        }, options);


        if (Dialog == null)
            return false;

        return (bool)Dialog;
    }
}
