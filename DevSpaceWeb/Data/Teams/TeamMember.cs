using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using MongoDB.Driver;
using Newtonsoft.Json;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceShared.Data;

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

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out var uovr) && uovr.LogPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.LogPermissions.HasFlag(permission))
                    return true;

            }

            if (log.RolePermissionOverrides.TryGetValue(r, out var ovr) && ovr.LogPermissions.HasFlag(permission))
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

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out var uovr) && uovr.ProjectPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.ProjectPermissions.HasFlag(permission))
                    return true;

            }

            if (project.RolePermissionOverrides.TryGetValue(r, out var ovr) && ovr.ProjectPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public bool HasServerPermission(ServerData server, ServerPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Server.ServerAdministrator)
            return true;

        if (Perms.Server.Has(permission))
            return true;

        if (server.MemberPermissionOverrides.TryGetValue(server.Id, out var uovr) && uovr.ServerPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.ServerPermissions.HasFlag(permission))
                    return true;

            }

            if (server.RolePermissionOverrides.TryGetValue(r, out var ovr) && ovr.ServerPermissions.HasFlag(permission))
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

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out var uovr) && uovr.WebsitePermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.WebsitePermissions.HasFlag(permission))
                    return true;

            }

            if (website.RolePermissionOverrides.TryGetValue(r, out var ovr) && ovr.WebsitePermissions.HasFlag(permission))
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

        if (server.MemberPermissionOverrides.TryGetValue(server.Id, out var uovr) && uovr.DockerPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.Permissions.DockerPermissions.HasFlag(permission))
                    return true;

            }

            if (server.RolePermissionOverrides.TryGetValue(r, out var ovr) && ovr.DockerPermissions.HasFlag(permission))
                return true;
        }

        return false;
    }

    public int GetRank()
    {
        //if (Team.OwnerId == UserId)
        //    return -1;

        int CurrentRank = int.MaxValue;
        foreach(var r in Roles)
        {
            if (Team.CachedRoles.TryGetValue(r, out var role) && role.Position < CurrentRank)
            {
                CurrentRank = role.Position;
            }
        }
        return CurrentRank;
    }

    public HashSet<ObjectId> Roles { get; set; } = new HashSet<ObjectId>();
    public string NickName { get; set; }
    public Guid? AvatarId { get; set; }

    public async Task UpdateAsync(UpdateDefinition<TeamMemberData> update)
    {
        FilterDefinition<TeamMemberData> filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Members.Collection.UpdateOneAsync(filter, update);
        if (!Result.IsAcknowledged)
            throw new InvalidOperationException("Failed to update member data");
    }
}
