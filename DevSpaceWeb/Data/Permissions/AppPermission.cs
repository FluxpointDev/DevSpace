namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum AppPermission : ulong
{
    ViewApp = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageApp = 1L << 3,
    AppAdministrator = 1L << 4,
    CreateAppResource = 1L << 5,
    DeleteAppResource = 1L << 6,
    ManageInstall = 1L << 7,
    ViewCommands = 1L << 8,
    ManageCommands = 1L << 9,
    ViewWorkspaces = 1L << 10,
    ManageWorkspaces = 1L << 11,
    ViewLogs = 1L << 12,
}
