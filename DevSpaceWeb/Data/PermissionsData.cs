using DevSpaceWeb.Components.Pages.Teams;
using DevSpaceWeb.Data.Permissions;
using Radzen.Blazor;

namespace DevSpaceWeb.Data;

public static class PermissionsData
{
    public static PermissionItem<TeamPermission>[] AllTeamData = GetTeamData();
    public static PermissionItem<ServerPermission>[] AllServerData = GetServerData(true);
    public static PermissionItem<WebsitePermission>[] AllWebsiteData = GetWebsiteData(true);
    public static PermissionItem<ProjectPermission>[] AllProjectData = GetProjectData(true);
    public static PermissionItem<LogPermission>[] AllLogData = GetLogData(true);
    public static PermissionItem<DockerPermission>[] AllDockerData = GetDockerData(true);

    public static PermissionItem<ServerPermission>[] ResourceServerData = GetServerData(false);
    public static PermissionItem<WebsitePermission>[] ResourceWebsiteData = GetWebsiteData(false);
    public static PermissionItem<ProjectPermission>[] ResourceProjectData = GetProjectData(false);
    public static PermissionItem<LogPermission>[] ResourceLogData = GetLogData(false);
    public static PermissionItem<DockerPermission>[] ResourceDockerData = GetDockerData(false);

    private static PermissionItem<TeamPermission>[] GetTeamData()
    {
        return new PermissionItem<TeamPermission>[]
        {
            new PermissionItem<TeamPermission>(TeamPermission.ViewMembers, "View Members", "Allows you to view members in the team"),
            new PermissionItem<TeamPermission>(TeamPermission.ManageMembers, "Manage Members", "Add or remove members to the team except global admins."),
            new PermissionItem<TeamPermission>(TeamPermission.AssignRoles, "Assign Roles", "Give members roles."),
            new PermissionItem<TeamPermission>(TeamPermission.ManageRoles, "Manage Roles", "Create, delete and edit roles."),
            new PermissionItem<TeamPermission>(TeamPermission.ViewPermissions, "View Permissions", "Allows you to view all the team permissions."),
            new PermissionItem<TeamPermission>(TeamPermission.ManagePermissions, "Manage Permissions", "Change permissions for roles except global admin."),
            new PermissionItem<TeamPermission>(TeamPermission.ManageTeam, "Manage Team", "Change team settings."),
            new PermissionItem<TeamPermission>(TeamPermission.ManageResources, "Manage All Resources", "Add or delete resources for the team such as servers/websites/projects."),
            new PermissionItem<TeamPermission>(TeamPermission.ViewAuditLogs, "View Audit Logs", "View records for team and resource access and settings changes."),
            new PermissionItem<TeamPermission>(TeamPermission.ViewAuditLogs, "View Audit Logs", "View records for team and resource access and settings changes."),
            new PermissionItem<TeamPermission>(TeamPermission.ViewAPIs, "View APIs", "View all API clients."),
            new PermissionItem<TeamPermission>(TeamPermission.ManageOwnAPIs, "Manage Own APIs", "Create and manage your own API clients."),
            new PermissionItem<TeamPermission>(TeamPermission.ManageAllAPIs, "Manage All APIs", "Create and manage all API clients."),
        };
    }

    private static PermissionItem<ServerPermission>[] GetServerData(bool plural)
    {
        return new PermissionItem<ServerPermission>[]
        {
            new PermissionItem<ServerPermission>(ServerPermission.ViewServer, "View Server" + (plural ? "s" : ""), "View all servers the team owns."),
            new PermissionItem<ServerPermission>(ServerPermission.ManageServer, "Manage Server" + (plural ? "s" : ""), "Change server settings."),
            new PermissionItem<ServerPermission>(ServerPermission.ManagePermissions, "Manage Permissions", "Change permissions for all servers the team owns."),
            new PermissionItem<ServerPermission>(ServerPermission.ManageResource, plural ? "Manage Server Resources" : "Manage Resource", "Add or delete servers for the team."),
        };
    }

    private static PermissionItem<WebsitePermission>[] GetWebsiteData(bool plural)
    {
        return new PermissionItem<WebsitePermission>[]
            {
            new PermissionItem<WebsitePermission>(WebsitePermission.ViewWebsite, "View Website" + (plural ? "s" : ""), "View all websites the team owns."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ManageWebsite, "Manage Website" + (plural ? "s" : ""), "Change website settings."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ManagePermissions, "Manage Permissions", "Change permissions for all websites the team owns."),
            new PermissionItem<WebsitePermission>(WebsitePermission.ManageResource, plural ? "Manage Website Resources" : "Manage Resource", "Add or delete websites for the team."),
            };
    }

    private static PermissionItem<ProjectPermission>[] GetProjectData(bool plural)
    {
        return new PermissionItem<ProjectPermission>[]
        {
            new PermissionItem<ProjectPermission>(ProjectPermission.ViewProject, "View Project" + (plural ? "s" : ""), "View all projects the team owns."),
           new PermissionItem<ProjectPermission>(ProjectPermission.ManageProject, "Manage Project" + (plural ? "s" : ""), "Change project settings."),
            new PermissionItem<ProjectPermission>(ProjectPermission.ManagePermissions, "Manage Permissions", "Change permissions for all projects the team owns."),
            new PermissionItem<ProjectPermission>(ProjectPermission.ManageResource, plural ? "Manage Project Resources" : "Manage Resource", "Add or delete projects for the team."),
        };
    }

    private static PermissionItem<LogPermission>[] GetLogData(bool plural)
    {
        return new PermissionItem<LogPermission>[]
        {
            new PermissionItem<LogPermission>(LogPermission.ViewLog, "View Log" + (plural ? "s" : ""), "View all logs the team owns."),
            new PermissionItem<LogPermission>(LogPermission.ManageLog, "Manage Log" + (plural ? "s" : ""), "Change log settings."),
            new PermissionItem<LogPermission>(LogPermission.ManagePermissions, "Manage Permissions", "Change permissions for all logs the team owns."),
            new PermissionItem<LogPermission>(LogPermission.ManageResource, plural ? "Manage Log Resources" : "Manage Resource", "Add or delete logs for the team."),
        };
    }

    private static PermissionItem<DockerPermission>[] GetDockerData(bool plural)
    {
        return new PermissionItem<DockerPermission>[]
            {
                new PermissionItem<DockerPermission>(DockerPermission.ViewContainers, "View Container" + (plural ? "s" : ""), "View all docker stacks and containers the team owns."),
                new PermissionItem<DockerPermission>(DockerPermission.ControlContainers, "Control Container" + (plural ? "s" : ""), "Start, stop, restart, pause and kill containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageContainers, "Manage Container" + (plural ? "s" : ""), "Create and modify Stacks and containers."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewImages, "View Images", "View current docker images."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageImages, "Manage Images", "Create and modify docker images."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewVolumes, "View Volumes", "View current docker volumes/storage."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageVolumes, "Manage Volumes", "Create and modify docker volumes."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewNetworks, "View Networks", "View current docker networks."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageNetworks, "Manage Networks", "Create and modify docker networks."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewTemplates, "View Templates", "View a list bunch of premade docker templates."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewCustomTemplates, "View Custom Templates", "View a list of custom docker templates made by other members."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageCustomTemplates, "Manage Custom Templates", "Create and modify custom docker templates."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewPlugins, "View Plugins", "View current docker templates installed."),
                new PermissionItem<DockerPermission>(DockerPermission.ManagePlugins, "Manage Plugins", "Install docker plugins on the server."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewEvents, "View Events", "View realtime docker events for all stacks, containers, images, networks, volumes and plugins."),
                new PermissionItem<DockerPermission>(DockerPermission.ViewRegistries, "View Registry", "View current docker registries."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageRegistries, "Manage Registry", "Create and modify docker registries."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageSettings, "Manage Settings", "Modify server-wide docker settings and features."),
                new PermissionItem<DockerPermission>(DockerPermission.ManageStackPermissions, "Manage Stack Permissions", "Manage permissions for stacks."),
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
    All, Server, Docker, Website, Project, Log
}