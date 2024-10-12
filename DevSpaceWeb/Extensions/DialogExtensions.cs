using DevSpaceWeb.Components.Dialogs;
using DevSpaceWeb.Models.Teams;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DevSpaceWeb;

public static class DialogExtensions
{
    public static async Task<bool> ShowConfirmAsync(this DialogService service, string title, string text, string buttonText, ButtonStyle color)
    {
        var options = new DialogOptions() { };

        var Dialog = await service.OpenAsync<ConfirmInfoDialog>(title, new Dictionary<string, object>()
        {
            { "ContentText", text },
            { "ButtonText", buttonText },
            { "Color", color }
        }, options);


        if (Dialog == null)
            return false;

        return (bool)Dialog;
    }

    public static async Task<bool> ShowDynamicForm<Model>(this DialogService service, string title, object model, Func<object, string?> function)
    {
        var options = new DialogOptions() { AutoFocusFirstElement = true };

        var dialog = await service.OpenAsync<DynamicFormDialog>("Test", new Dictionary<string, object>()
        {
            { "ModelData", model },
            { "SubmitTask", function
            }

        }, options);
        return dialog;
    }

    public static async Task<bool> ShowInfoAsync(this DialogService service, string title, string text)
    {
        var options = new DialogOptions() { };

        var Dialog = await service.OpenAsync<ConfirmInfoDialog>(title, new Dictionary<string, object>()
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
