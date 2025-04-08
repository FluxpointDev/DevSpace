using DevSpaceWeb.Data.Permissions;
using Radzen;

namespace DevSpaceWeb;

public static class NotificationExtensions
{
    public static void ShowPermissionWarning(this NotificationService notify, TeamPermission permission)
        => ShowPermissionWarningInternal(notify, Utils.FriendlyName(permission.ToString()));

    public static void ShowPermissionWarning(this NotificationService notify, ServerPermission permission)
        => ShowPermissionWarningInternal(notify, Utils.FriendlyName(permission.ToString()));
    public static void ShowPermissionWarningInternal(NotificationService notify, string permission)
    {
        notify.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Duration = 40000,
            Summary = "Permission Error",
            Detail = $"You do not have permission for {permission}"

        });
    }



    public static void ShowErrorWarning(this NotificationService notify, NotificationErrorType error)
    {
        string Message = "";
        switch (error)
        {
            case NotificationErrorType.AccountLoadFailed:
                Message = "Failed to get account.";
                break;
            case NotificationErrorType.PreviewNotEnabled:
                Message = "Preview mode is not enabled.";
                break;
            case NotificationErrorType.AuthenticatorNoDevices:
                Message = "You do not have any authenticator devices linked to your account.";
                break;
            case NotificationErrorType.PasskeysNoDevices:
                Message = "You do not have any passkey devices linked to your account.";
                break;
            case NotificationErrorType.UploadNoFile:
                Message = "You need to add a file to upload.";
                break;
        }
        notify.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Duration = 40000,
            Summary = "Error",
            Detail = Message

        });
    }


}
public enum NotificationErrorType
{
    AccountLoadFailed, PreviewNotEnabled, AuthenticatorNoDevices, PasskeysNoDevices, UploadNoFile
}