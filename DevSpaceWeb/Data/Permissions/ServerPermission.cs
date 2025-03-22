namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ServerPermission : ulong
{
    ViewServer = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageServer = 1L << 3,
    ServerAdministrator = 1L << 4,
    CreateServerResource = 1L << 5,
    DeleteServerResource = 1L << 6,
    ManageConnection = 1L << 7,
    ViewHostInfo = 1L << 8,
}
