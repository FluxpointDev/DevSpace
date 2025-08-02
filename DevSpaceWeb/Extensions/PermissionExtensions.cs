using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;
using MongoDB.Bson;

namespace DevSpaceWeb;

public static class PermissionExtensions
{
    public static bool HasTeamPermission(this TeamMemberData? member, TeamData? team, TeamPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasTeamPermission(checkPermission))
            return true;

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasTeamPermission(team, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasAPIPermission(this TeamMemberData? member, TeamData? team, APIPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasAPIPermission(APIPermission.APIAdministrator))
            return true;

        if (team.DefaultPermissions.HasAPIPermission(checkPermission))
            return true;

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasAPIPermission(team, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasLogPermission(this TeamMemberData? member, TeamData? team, ProjectData? project, LogPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (project != null)
        {
            if (project.TeamId != member.TeamId || project.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasLogPermission(checkPermission))
            return true;

        if (project != null)
        {
            if (project.DefaultPermissions.HasLogPermission(checkPermission))
                return true;

            if (project.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasLogPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasLogPermission(team, project, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasProjectPermission(this TeamMemberData? member, TeamData? team, ProjectData? project, ProjectPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (project != null)
        {
            if (project.TeamId != member.TeamId || project.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasProjectPermission(checkPermission))
            return true;

        if (project != null)
        {
            if (project.DefaultPermissions.HasProjectPermission(checkPermission))
                return true;

            if (project.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasProjectPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasProjectPermission(team, project, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasServerPermission(this TeamMemberData? member, TeamData? team, ServerData? server, ServerPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (server != null)
        {
            if (server.TeamId != member.TeamId || server.TeamId != team.Id)
                return false;
        }

        if (member.TeamId != team.Id)
            return false;

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasServerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasServerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasServerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasServerPermission(team, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasWebsitePermission(this TeamMemberData? member, TeamData? team, WebsiteData? website, WebsitePermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (website != null)
        {
            if (website.TeamId != member.TeamId || website.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasWebsitePermission(checkPermission))
            return true;

        if (website != null)
        {
            if (website.DefaultPermissions.HasWebsitePermission(checkPermission))
                return true;

            if (website.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasWebsitePermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasWebsitePermission(team, website, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasAppPermission(this TeamMemberData? member, TeamData? team, AppData? app, AppPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (app != null)
        {
            if (app.TeamId != member.TeamId || app.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasAppPermission(checkPermission))
            return true;

        if (app != null)
        {
            if (app.DefaultPermissions.HasAppPermission(checkPermission))
                return true;

            if (app.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasAppPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasAppPermission(team, app, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasConsolePermission(this TeamMemberData? member, TeamData? team, ConsoleData? console, ConsolePermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (console != null)
        {
            if (console.TeamId != member.TeamId || console.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasConsolePermission(checkPermission))
            return true;

        if (console != null)
        {
            if (console.DefaultPermissions.HasConsolePermission(checkPermission))
                return true;

            if (console.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasConsolePermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasConsolePermission(team, console, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasDockerPermission(this TeamMemberData? member, TeamData? team, ServerData? server, DockerPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (server != null)
        {
            if (server.TeamId != member.TeamId || server.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasDockerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasDockerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasDockerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerPermission(team, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public static bool HasDockerContainerPermission(this TeamMemberData? member, TeamData? team, ServerData? server, DockerContainerPermission checkPermission)
    {
        if (team == null || member == null)
            return false;

        if (member.TeamId != team.Id)
            return false;

        if (server != null)
        {
            if (server.TeamId != member.TeamId || server.TeamId != team.Id)
                return false;
        }

        if (team.OwnerId == member.UserId)
            return true;

        if (team.DefaultPermissions.HasDockerContainerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasDockerContainerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(member.UserId, out PermissionsSet? uovr) && uovr.HasDockerContainerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in member.Roles)
        {
            if (team.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerContainerPermission(team, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

}
