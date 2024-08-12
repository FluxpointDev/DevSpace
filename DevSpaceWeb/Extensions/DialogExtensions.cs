using DevSpaceWeb.Components.Dialogs;
using MudBlazor;

namespace DevSpaceWeb;

public static class DialogExtensions
{
    public static async Task<DialogResult?> ShowConfirmAsync(this IDialogService service, string title, string text, string buttonText, Color color)
    {
        var parameters = new DialogParameters<ConfirmInfoDialog>
        {
            { x => x.ContentText, text },
            { x => x.ButtonText, buttonText },
            { x => x.Color, color }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var Dialog = await service.ShowAsync<ConfirmInfoDialog>(title, parameters, options);
        if (Dialog == null)
            return null;

        var Result = await Dialog.Result;

        return Result;
    }

    public static async Task<DialogResult?> ShowInfoAsync(this IDialogService service, string title, string text)
    {
        var parameters = new DialogParameters<ConfirmInfoDialog>
        {
            { x => x.ContentText, text },
            { x => x.ButtonText, "Ok" },
            { x => x.Color, Color.Success },
            { x => x.ShowCancel, false }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var Dialog = await service.ShowAsync<ConfirmInfoDialog>(title, parameters, options);
        if (Dialog == null)
            return null;

        var Result = await Dialog.Result;

        return Result;
    }
}
