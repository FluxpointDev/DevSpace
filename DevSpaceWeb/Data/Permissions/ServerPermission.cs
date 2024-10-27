namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ServerPermission : ulong
{
    ViewServer = 1L << 0,
    ManagePermissions = 1L << 1,
    ServerAdministrator = 1L << 2,
    DockerController = 1L << 3,
    DockerManager = 1L << 4,
    DockerAdministrator = 1L << 5,
}
