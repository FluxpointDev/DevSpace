namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ResourcePermission : ulong
{
    CreateServers,
    CreateConsoles,
    CreateWebsites,
    CreateProjects,
    REDACTED,
    CreateAPIs,
    CreateApps,
}
