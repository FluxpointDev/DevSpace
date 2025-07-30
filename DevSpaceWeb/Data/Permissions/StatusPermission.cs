namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum StatusPermission : ulong
{
    ViewMonitors = 1L << 0,
    ViewPages = 1L << 1,
    CreateMonitors = 1L << 2,
    CreatePages = 1L << 3,
}
