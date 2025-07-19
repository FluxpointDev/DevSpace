namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ProjectPermission : ulong
{
    ViewProject = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageProject = 1L << 3,
    ProjectAdministrator = 1L << 4,
    CreateProjectResource = 1L << 5,
    DeleteProjectResource = 1L << 6
}
