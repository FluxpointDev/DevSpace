namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum StatusMonitorPermission : ulong
{
    ViewMonitor = 1L << 0,
    CreateMonitorResource = 1L << 1,
    MonitorAdministrator = 1L << 2,
    ViewPermissions = 1L << 3,
    ManagePermissions = 1L << 4,
    DeleteMonitorResource = 1L << 5,
    ManageMonitor = 1L << 6,
}
[Flags]
public enum StatusPagePermission : ulong
{
    ViewPage = 1L << 0,
    CreatePageResource = 1L << 1,
    PageAdministrator = 1L << 2,
    ViewPermissions = 1L << 3,
    ManagePermissions = 1L << 4,
    DeletePageResource = 1L << 5,
    ManagePage = 1L << 6,
}
[Flags]
public enum StatusIssuePermission : ulong
{
    ViewIssue = 1L << 0,
    CreateIssueResource = 1L << 1,
    IssueAdministrator = 1L << 2,
    ViewPermissions = 1L << 3,
    ManagePermissions = 1L << 4,
    DeleteIssueResource = 1L << 5,
    ManageIssue = 1L << 6,
}