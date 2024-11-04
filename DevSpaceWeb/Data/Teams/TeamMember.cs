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

namespace DevSpaceWeb.Data.Teams;

public class TeamMemberData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId UserId { get; set; }
    public ObjectId TeamId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId.ToString());

    public bool HasTeamPermission(TeamPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsAll Perms = SelectedTeam.GetPermissions();

        if (Perms.Team.GlobalAdministrator || Perms.Team.TeamAdministrator)
            return true;

        if (Perms.Team.Has(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role) && role.HasTeamPermission(this, permission))
                return true;
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

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out var role) && role.HasLogPermission(this, permission))
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

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out var role) && role.HasProjectPermission(this, permission))
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

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out var role) && role.HasServerPermission(this, permission))
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

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out var role) && role.HasWebsitePermission(this, permission))
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

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out var role) && role.HasDockerPermission(this, permission))
                return true;
        }

        return false;
    }

    public HashSet<ObjectId> Roles { get; set; } = new HashSet<ObjectId>();
    public string NickName { get; set; }
    public Guid? AvatarId { get; set; }

    public void Update(UpdateDefinition<TeamMemberData> update)
    {
        FilterDefinition<TeamMemberData> filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
        _DB.Members.Collection.UpdateOne(filter, update);
    }
}
