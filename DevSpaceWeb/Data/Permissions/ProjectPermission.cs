namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ProjectPermission : ulong
{
    ViewProject = 1L << 0,
    ManagePermissions = 1L << 1,
    ProjectAdministrator = 1L << 2,
}
