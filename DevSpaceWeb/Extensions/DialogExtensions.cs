using DevSpaceWeb.Components.Dialogs;
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

    public static async Task<bool> ShowInfoAsync(this DialogService service, string title, string text)
    {
        var options = new DialogOptions() { };

        var Dialog = await service.OpenAsync<ConfirmInfoDialog>(title, new Dictionary<string, object>()
        {
            { "ContentText", text },
            { "ButtonText", "Ok" },
            { "Color", Radzen.Colors.Success },
            { "ShowCancel", false }
        }, options);


        if (Dialog == null)
            return false;

        return (bool)Dialog;
    }
}
