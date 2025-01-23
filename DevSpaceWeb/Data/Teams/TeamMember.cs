using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamMemberData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId UserId { get; set; }
    public ObjectId TeamId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public UserDisabled? Disabled { get; set; }

    public bool HasTeamPermission(TeamPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator)
            return true;

        if (Perms.Team.Has(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.TeamPermissions.HasFlag(permission))
                    return true;
            }
        }

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Log.LogAdministrator)
            return true;

        if (Perms.Log.Has(permission))
            return true;

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out PermissionsSet? uovr) && uovr.LogPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.LogPermissions.HasFlag(permission))
                    return true;

            }

            if (log.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.LogPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Project.ProjectAdministrator)
            return true;

        if (Perms.Project.Has(permission))
            return true;

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out PermissionsSet? uovr) && uovr.ProjectPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.ProjectPermissions.HasFlag(permission))
                    return true;

            }

            if (project.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.ProjectPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasServerPermission(ServerData? server, ServerPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Server.ServerAdministrator)
            return true;

        if (permission == ServerPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (Perms.Server.Has(permission))
            return true;

        if (server != null && server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.ServerPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.ServerPermissions.HasFlag(permission))
                    return true;

            }

            if (server != null && server.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.ServerPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Website.WebsiteAdministrator)
            return true;

        if (Perms.Website.Has(permission))
            return true;

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out PermissionsSet? uovr) && uovr.WebsitePermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.WebsitePermissions.HasFlag(permission))
                    return true;

            }

            if (website.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.WebsitePermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasConsolePermission(ConsoleData? console, ConsolePermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Console.ConsoleAdministrator)
            return true;

        if (Perms.Console.Has(permission))
            return true;

        if (console != null && console.MemberPermissionOverrides.TryGetValue(console.Id, out PermissionsSet? uovr) && uovr.ConsolePermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.ConsolePermissions.HasFlag(permission))
                    return true;

            }

            if (console != null && console.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.ConsolePermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Docker.DockerAdministrator)
            return true;

        if (Perms.Docker.Has(permission))
            return true;

        if (server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.DockerPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.DockerPermissions.HasFlag(permission))
                    return true;

            }

            if (server.RolePermissionOverrides.TryGetValue(r, out PermissionsSet? ovr) && ovr.DockerPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public int GetRank()
    {
        if (Team.OwnerId == UserId)
            return int.MaxValue;

        int CurrentRank = -1;
        foreach (ObjectId r in Roles)
        {
            if (Team.CachedRoles.TryGetValue(r, out TeamRoleData? role) && role.Position > CurrentRank)
            {
                CurrentRank = role.Position;
            }
        }
        return CurrentRank;
    }

    public HashSet<ObjectId> Roles { get; set; } = new HashSet<ObjectId>();
    public string? NickName { get; set; }

    public Guid? AvatarId { get; set; }

    public async Task UpdateAsync(UpdateDefinition<TeamMemberData> update, Action action)
    {
        FilterDefinition<TeamMemberData> filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Members.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
