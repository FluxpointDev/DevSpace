namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ProjectPermission : ulong
{
    ViewProject = 1L << 0,
    ManagePermissions = 1L << 1,
    ManageProject = 1L << 2,
    ProjectAdministrator = 1L << 3,
    ManageResource = 1L << 4,
}
