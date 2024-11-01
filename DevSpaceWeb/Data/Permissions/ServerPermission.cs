namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ServerPermission : ulong
{
    ViewServer = 1L << 0,
    ManagePermissions = 1L << 1,
    ServerAdministrator = 1L << 2
}
