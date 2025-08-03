using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Status;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;

namespace DevSpaceWeb;

public static class RolePermissionExtensions
{
    public static bool HasTeamPermission(this TeamRoleData? role, TeamData? team, TeamPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (role.Permissions.HasTeamPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasAPIPermission(this TeamRoleData? role, TeamData? team, APIPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (role.Permissions.HasAPIPermission(APIPermission.APIAdministrator))
            return true;

        if (role.Permissions.HasAPIPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasLogPermission(this TeamRoleData? role, TeamData? team, ProjectData? project, LogPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (project != null && project.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasLogPermission(checkPermission))
            return true;

        if (role.Permissions.HasLogPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasProjectPermission(this TeamRoleData? role, TeamData? team, ProjectData? project, ProjectPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (project != null && project.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasProjectPermission(checkPermission))
            return true;

        if (role.Permissions.HasProjectPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasAppPermission(this TeamRoleData? role, TeamData? team, AppData? app, AppPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (app != null && app.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasAppPermission(checkPermission))
            return true;

        if (role.Permissions.HasAppPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasServerPermission(this TeamRoleData? role, TeamData? team, ServerData? server, ServerPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasServerPermission(checkPermission))
            return true;

        if (role.Permissions.HasServerPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasConsolePermission(this TeamRoleData? role, TeamData? team, ConsoleData? console, ConsolePermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (console != null && console.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasConsolePermission(checkPermission))
            return true;

        if (role.Permissions.HasConsolePermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasWebsitePermission(this TeamRoleData? role, TeamData? team, WebsiteData? website, WebsitePermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (website != null && website.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasWebsitePermission(checkPermission))
            return true;

        if (role.Permissions.HasWebsitePermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasDockerPermission(this TeamRoleData? role, TeamData? team, ServerData? server, DockerPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasDockerPermission(checkPermission))
            return true;

        if (role.Permissions.HasDockerPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasDockerContainerPermission(this TeamRoleData? role, TeamData? team, ServerData? server, DockerContainerPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasDockerContainerPermission(checkPermission))
            return true;

        if (role.Permissions.HasDockerContainerPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasStatusMonitorPermission(this TeamRoleData? role, TeamData? team, StatusMonitorData? monitor, StatusMonitorPermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (monitor != null && monitor.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasStatusMonitorPermission(checkPermission))
            return true;

        if (role.Permissions.HasStatusMonitorPermission(checkPermission))
            return true;

        return false;
    }

    public static bool HasStatusPagePermission(this TeamRoleData? role, TeamData? team, StatusPageData? page, StatusPagePermission checkPermission)
    {
        if (team == null || role == null || team.Id != role.TeamId)
            return false;

        if (page != null && page.RolePermissionOverrides.TryGetValue(role.Id, out PermissionsSet? perms) && perms.HasStatusPagePermission(checkPermission))
            return true;

        if (role.Permissions.HasStatusPagePermission(checkPermission))
            return true;

        return false;
    }
}
