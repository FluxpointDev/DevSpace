namespace DevSpaceWeb.Apps.Data;

[Flags]
public enum PermissionFlag : ulong
{
    PermissionsOverride = 1 << 0,
    PermissionsManage = 1 << 1,
    PermissionsView = 1 << 2,
    AppManage = 1 << 3,
    AppUpdateToken = 1 << 4,
    AppModifyInstall = 1 << 5,
    AppView = 1 << 6,
    SubscriptionManage = 1 << 7,
    SubscriptionView = 1 << 8,
    ConfigManage = 1 << 9,
    ConfigView = 1 << 10,
    TranslationManage = 1 << 11,
    TranslationView = 1 << 12,
    WorkspaceManage = 1 << 13,
    WorkspaceEdit = 1 << 14,
    WorkspaceView = 1 << 15,
}