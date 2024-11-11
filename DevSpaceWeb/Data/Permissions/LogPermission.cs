namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum LogPermission : ulong
{
    ViewLog = 1L << 0,
    ManagePermissions = 1L << 1,
    ManageLog = 1L << 2,
    LogAdministrator = 1L << 3,
}
