using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamRoleData : IObject
{
    public ObjectId TeamId { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Color { get; set; }

    [Obsolete]
    [BsonIgnoreIfNull]
    public int? Position { get; set; }

    public PermissionsSet Permissions { get; set; } = new PermissionsSet();

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public int GetPosition()
    {
        if (Team != null && Team.RolePositions.TryGetValue(Id, out int position))
            return position;

        return 0;
    }

    public bool CanManage(TeamMemberData? currentMember)
    {
        if (currentMember == null || TeamId != currentMember.TeamId)
            return false;

        if (Team != null && Team.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > GetPosition())
            return true;
        return false;
    }

    public bool HasTeamPermission(TeamData? team, TeamPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (Permissions.HasTeamPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasAPIPermission(TeamData? team, APIPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (Permissions.HasAPIPermission(APIPermission.APIAdministrator))
            return true;

        if (Permissions.HasAPIPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasLogPermission(TeamData? team, ProjectData? project, LogPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (project != null && project.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasLogPermission(checkPermission))
            return true;

        if (Permissions.HasLogPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasProjectPermission(TeamData? team, ProjectData? project, ProjectPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (project != null && project.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasProjectPermission(checkPermission))
            return true;

        if (Permissions.HasProjectPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasAppPermission(TeamData? team, AppData? app, AppPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (app != null && app.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasAppPermission(checkPermission))
            return true;

        if (Permissions.HasAppPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasServerPermission(TeamData? team, ServerData? server, ServerPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasServerPermission(checkPermission))
            return true;

        if (Permissions.HasServerPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasConsolePermission(TeamData? team, ConsoleData? console, ConsolePermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (console != null && console.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasConsolePermission(checkPermission))
            return true;

        if (Permissions.HasConsolePermission(checkPermission))
            return true;

        return false;
    }

    public bool HasWebsitePermission(TeamData? team, WebsiteData? website, WebsitePermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (website != null && website.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasWebsitePermission(checkPermission))
            return true;

        if (Permissions.HasWebsitePermission(checkPermission))
            return true;

        return false;
    }

    public bool HasDockerPermission(TeamData? team, ServerData? server, DockerPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasDockerPermission(checkPermission))
            return true;

        if (Permissions.HasDockerPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasDockerContainerPermission(TeamData? team, ServerData? server, DockerContainerPermission checkPermission)
    {
        if (team == null || team.Id != TeamId)
            return false;

        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasDockerContainerPermission(checkPermission))
            return true;

        if (Permissions.HasDockerContainerPermission(checkPermission))
            return true;

        return false;
    }

    public async Task UpdateAsync(UpdateDefinition<TeamRoleData> update, Action action)
    {
        FilterDefinition<TeamRoleData> filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Roles.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}