namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ServerPermission : ulong
{
    ViewServer = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageServer = 1L << 3,
    ServerAdministrator = 1L << 4,
    ManageResource = 1L << 5,
}
