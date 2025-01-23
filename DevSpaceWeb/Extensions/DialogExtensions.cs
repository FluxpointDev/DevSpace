using DevSpaceWeb.Components.Dialogs;
using DevSpaceWeb.Data.Permissions;
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

    public static Task ShowPermissionErrorAsync(this DialogService service, TeamPermission flag)
        => InternalShowPermissionErrorAsync(service, flag);

    public static Task ShowPermissionErrorAsync(this DialogService service, ServerPermission flag)
        => InternalShowPermissionErrorAsync(service, flag);

    public static Task ShowPermissionErrorAsync(this DialogService service, ConsolePermission flag)
        => InternalShowPermissionErrorAsync(service, flag);

    private static async Task InternalShowPermissionErrorAsync(this DialogService service, Enum flag)
    {
        DialogOptions options = new DialogOptions() { };

        dynamic Dialog = await service.OpenAsync<ConfirmInfoDialog>("Permission Error", new Dictionary<string, object>()
        {
            { "ContentText", $"You do not have permission for {Utils.FriendlyName(flag.ToString())}" },
            { "ButtonText", "Ok" },
            { "Color", ButtonStyle.Success },
            { "ShowCancel", false }
        }, options);
    }
}
