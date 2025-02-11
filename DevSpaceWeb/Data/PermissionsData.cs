using DevSpaceWeb.Components.Pages.Teams;
using DevSpaceWeb.Data.Permissions;
using Radzen.Blazor;

namespace DevSpaceWeb.Data;

public static class PermissionsData
{
    public static PermissionItem<TeamPermission>[] AllTeamData = GetTeamData();
    public static PermissionItem<APIPermission>[] AllApiData = GetApiData();
    public static PermissionItem<ServerPermission>[] AllServerData = GetServerData(true);
    public static PermissionItem<WebsitePermission>[] AllWebsiteData = GetWebsiteData(true);
    public static PermissionItem<ProjectPermission>[] AllProjectData = GetProjectData(true);
    public static PermissionItem<LogPermission>[] AllLogData = GetLogData(true);
    public static PermissionItem<DockerPermission>[] AllDockerData = GetDockerData(true);
    public static PermissionItem<ConsolePermission>[] AllConsoleData = GetConsoleData(true);

    public static PermissionItem<ServerPermission>[] ResourceServerData = GetServerData(false);
    public static PermissionItem<WebsitePermission>[] ResourceWebsiteData = GetWebsiteData(false);
    public static PermissionItem<ProjectPermission>[] ResourceProjectData = GetProjectData(false);
    public static PermissionItem<LogPermission>[] ResourceLogData = GetLogData(false);
    public static PermissionItem<DockerPermission>[] ResourceDockerData = GetDockerData(false);
    public static PermissionItem<ConsolePermission>[] ResourceConsoleData = GetConsoleData(false);

    private static PermissionItem<TeamPermission>[] GetTeamData()
    {
        return new PermissionItem<TeamPermission>[]
        {
            new PermissionItem<TeamPermission>(TeamPermission.ViewMembers, Lang.Permissions.view_members, Lang.Permissions.view_members_description),
            new PermissionItem<TeamPermission>(TeamPermission.ManageMembers, Lang.Permissions.manage_members, Lang.Permissions.manage_members_description),
            new PermissionItem<TeamPermission>(TeamPermission.ViewRoles, Lang.Permissions.view_roles, Lang.Permissions.view_roles_description),
            new PermissionItem<TeamPermission>(TeamPermission.AssignRoles, Lang.Permissions.assign_roles, Lang.Permissions.assign_roles_description),
            new PermissionItem<TeamPermission>(TeamPermission.ManageRoles, Lang.Permissions.manage_roles, Lang.Permissions.manage_roles_description),
            new PermissionItem<TeamPermission>(TeamPermission.ViewPermissions, Lang.Permissions.view_permissions, Lang.Permissions.view_permissions_description),
            new PermissionItem<TeamPermission>(TeamPermission.ManagePermissions, Lang.Permissions.manage_permissions, Lang.Permissions.manage_permissions_description),
            new PermissionItem<TeamPermission>(TeamPermission.ManageTeam, Lang.Permissions.manage_team, Lang.Permissions.manage_team_description),
            new PermissionItem<TeamPermission>(TeamPermission.ViewAuditLogs, Lang.Permissions.view_audit_logs, Lang.Permissions.view_audit_logs_description)
        };
    }

    private static PermissionItem<APIPermission>[] GetApiData()
    {
        return new PermissionItem<APIPermission>[]
        {
            new PermissionItem<APIPermission>(APIPermission.ViewOwnAPIs, Lang.Permissions.view_own_apis, Lang.Permissions.view_own_apis_description),
            new PermissionItem<APIPermission>(APIPermission.ManageOwnAPIs, Lang.Permissions.manage_own_apis, Lang.Permissions.manage_own_apis_description),
            new PermissionItem<APIPermission>(APIPermission.ViewAllAPIs, Lang.Permissions.view_all_apis, Lang.Permissions.view_all_apis_description),
        };
    }

    private static PermissionItem<ServerPermission>[] GetServerData(bool plural)
    {
        return new PermissionItem<ServerPermission>[]
        {
            new PermissionItem<ServerPermission>(ServerPermission.ManageResource, plural ? "Manage Server Resources" : "Manage Resource", plural ? "Add or delete servers for the team." : "Delete this resource."),
            new PermissionItem<ServerPermission>(ServerPermission.ManageServer, "Manage Server" + (plural ? "s" : ""), plural ? "Change all server settings." : "Change settings for this server."),
            new PermissionItem<ServerPermission>(ServerPermission.ViewPermissions, "View Permissions", "View permissions for the server."),
            new PermissionItem<ServerPermission>(ServerPermission.ManagePermissions, "Manage Permissions", "Change permissions for all servers the team owns."),
            new PermissionItem<ServerPermission>(ServerPermission.ManageConnection, "Manage Connection", "Update the server agent connection settings."),
        };
    }

    private static PermissionItem<WebsitePermission>[] GetWebsiteData(bool plural)
    {
        return new PermissionItem<WebsitePermission>[]
        {
            new PermissionItem<WebsitePermission>(WebsitePermission.ManageResource, plural ? "Manage Website Resources" : "Manage Resource", "Add or delete websites for the team."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ManageWebsite, "Manage Website" + (plural ? "s" : ""), "Change website settings."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ViewPermissions, "View Permissions", "View permissions for the website."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ManagePermissions, "Manage Permissions", "Change permissions for all websites the team owns."),

        };
    }

    private static PermissionItem<ProjectPermission>[] GetProjectData(bool plural)
    {
        return new PermissionItem<ProjectPermission>[]
        {
            new PermissionItem<ProjectPermission>(ProjectPermission.ManageResource, plural ? "Manage Project Resources" : "Manage Resource", "Add or delete projects for the team."),
            new PermissionItem<ProjectPermission>(ProjectPermission.ManageProject, "Manage Project" + (plural ? "s" : ""), "Change all project settings."),
            new PermissionItem<ProjectPermission>(ProjectPermission.ViewPermissions, "View Permissions", "View permissions for the project."),
            new PermissionItem<ProjectPermission>(ProjectPermission.ManagePermissions, "Manage Permissions", "Change permissions for all projects the team owns."),

        };
    }

    private static PermissionItem<LogPermission>[] GetLogData(bool plural)
    {
        return new PermissionItem<LogPermission>[]
        {
            new PermissionItem<LogPermission>(LogPermission.ManageResource, plural ? "Manage Log Resources" : "Manage Resource", "Add or delete logs for the team."),
            new PermissionItem<LogPermission>(LogPermission.ManageLog, "Manage Log" + (plural ? "s" : ""), "Change log settings."),
            new PermissionItem<LogPermission>(LogPermission.ViewPermissions, "View Permissions", "View permissions for the log."),
            new PermissionItem<LogPermission>(LogPermission.ManagePermissions, "Manage Permissions", "Change permissions for all logs the team owns."),

        };
    }

    private static PermissionItem<DockerPermission>[] GetDockerData(bool plural)
    {
        return new PermissionItem<DockerPermission>[]
            {
                new PermissionItem<DockerPermission>(DockerPermission.ViewContainers, "View Container" + (plural ? "s" : ""), "View all docker containers and stacks the team owns."),
                new PermissionItem<DockerPermission>(DockerPermission.ControlContainers, "Control Container" + (plural ? "s" : ""), "Start, stop, restart, pause and kill containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewImages, "View Images", "View docker images for the system."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewVolumes, "View Volumes", "View docker volumes/storage."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewNetworks, "View Networks", "View current docker networks."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewRegistries, "View Registry", "View current docker registries."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewContainerLogs, "View Container Logs", "View live container logs."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewContainerEnvironment, "View Container Environment", "View environment variables for containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewContainerFiles, "View Container Files", "View files inside the containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageContainerFiles, "Manage Container Files", "Create and modify files inside the containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewPermissions, "View Permissions", "View permissions for the containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ManagePermissions, "Manage Permissions", "Manage permissions for the containers.")
            };
    }

    private static PermissionItem<ConsolePermission>[] GetConsoleData(bool plural)
    {
        return new PermissionItem<ConsolePermission>[]
            {
                new PermissionItem<ConsolePermission>(ConsolePermission.ManageResource, "Manage Resource", "Manage all console resources for the team."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ManageConsole, "Manage Console" + (plural ? "s" : ""), "Manage all console settings for the team."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewPermissions, "View Permissions", "View permissions for the console"),
                new PermissionItem<ConsolePermission>(ConsolePermission.ManagePermissions, "Manage Permissions", "Change permissions for all consoles the team owns"),
                new PermissionItem<ConsolePermission>(ConsolePermission.UseConsoleCommands, "Use Console Commands", "Execute console commands on the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewPlayers, "View Players", "View all players on the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.KickPlayers, "Kick Players", "Kick players from the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.BanPlayers, "Ban Players", "Ban players from the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewBans, "View Bans", "View banned players."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ControlServer, "Control Server", "Restart, shutdown, lock or perform other actions on the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewConsoleLogs, "View Console Logs", "View console logs in realtime, may contain private/sensitive info."),
                new PermissionItem<ConsolePermission>(ConsolePermission.MessagePlayers, "Message Players", "Send private messages to players on the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.MessageGlobal, "Message Global", "Send global messages on the server."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ManageConnection, "Manage Connection", "Modify server/rcon connection settings."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewIPs, "View IPs", "View IPs from players and admins."),
                new PermissionItem<ConsolePermission>(ConsolePermission.ViewConnections, "View Connections", "View authorized rcon connections to the server."),
            };
    }
}
public class PermissionGrid<T>
{
    public RadzenDropDown<IEnumerable<T>> Grid;
    public T[] Data;

    public IEnumerable<T> Selected;
}
public enum PermissionType
{
    All, Server, Docker, Website, Project, Log, Console
}