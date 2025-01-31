using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamRoleData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId TeamId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Description { get; set; }

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

    public bool CanManage(TeamMemberData currentMember)
    {
        if (Team.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > GetPosition())
            return true;
        return false;
    }

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        if (Permissions.HasTeamPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasAPIPermission(APIPermission checkPermission)
    {
        if (Permissions.HasAPIPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission checkPermission)
    {
        if (log != null && log.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasLogPermission(checkPermission))
            return true;

        if (Permissions.HasLogPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission checkPermission)
    {
        if (project != null && project.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasProjectPermission(checkPermission))
            return true;

        if (Permissions.HasProjectPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasServerPermission(ServerData server, ServerPermission checkPermission)
    {
        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasServerPermission(checkPermission))
            return true;

        if (Permissions.HasServerPermission(checkPermission))
            return true;

        return false;
    }

    public bool HasConsolePermission(ConsoleData console, ConsolePermission checkPermission)
    {
        if (console != null && console.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasConsolePermission(checkPermission))
            return true;

        if (Permissions.HasConsolePermission(checkPermission))
            return true;

        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission checkPermission)
    {
        if (website != null && website.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasWebsitePermission(checkPermission))
            return true;

        if (Permissions.HasWebsitePermission(checkPermission))
            return true;

        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission checkPermission)
    {
        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasDockerPermission(checkPermission))
            return true;

        if (Permissions.HasDockerPermission(checkPermission))
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