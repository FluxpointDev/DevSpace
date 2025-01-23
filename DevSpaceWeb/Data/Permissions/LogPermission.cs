namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum LogPermission : ulong
{
    ViewLog = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageLog = 1L << 3,
    LogAdministrator = 1L << 4,
    ManageResource = 1L << 5,
}
