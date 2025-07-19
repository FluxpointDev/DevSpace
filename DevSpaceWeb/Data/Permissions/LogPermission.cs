namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum LogPermission : ulong
{
    ViewLogs = 1L << 0,
    ViewStacktrace = 1L << 1,
    ViewSensitiveInfo = 1L << 2,
    ViewComments = 1L << 3,
    CreateComments = 1L << 4,
    ViewReports = 1L << 5,
    ManageReports = 1L << 6,
    AssignTo = 1L << 7,
    ManageLogs = 1L << 8,
    LogAdministrator = 1L << 9,
    ViewContext = 1L << 10,
}