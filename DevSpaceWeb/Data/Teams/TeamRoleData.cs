using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
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
    public int Position { get; set; }
    public PermissionsSet Permissions { get; set; } = new PermissionsSet();

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public bool CanManage(TeamMemberData currentMember)
    {
        if (Team.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > Position)
            return true;
        return false;
    }

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (Permissions.TeamPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission checkPermission)
    {
        if (log != null && log.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.LogPermissions.HasFlag(LogPermission.LogAdministrator) || perms.LogPermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.LogPermissions.HasFlag(LogPermission.LogAdministrator))
            return true;

        if (Permissions.LogPermissions.HasFlag(checkPermission))
            return true;

        if (checkPermission == LogPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission checkPermission)
    {
        if (project != null && project.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator) || perms.ProjectPermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator))
            return true;

        if (Permissions.ProjectPermissions.HasFlag(checkPermission))
            return true;

        if (checkPermission == ProjectPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        return false;
    }

    public bool HasServerPermission(ServerData server, ServerPermission checkPermission)
    {
        if (server != null && server.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || perms.ServerPermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
            return true;

        if (Permissions.ServerPermissions.HasFlag(checkPermission))
            return true;

        if (checkPermission == ServerPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        return false;
    }

    public bool HasConsolePermission(ConsoleData console, ConsolePermission checkPermission)
    {
        if (console != null && console.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator) || perms.ConsolePermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator))
            return true;

        if (Permissions.ConsolePermissions.HasFlag(checkPermission))
            return true;

        if (checkPermission == ConsolePermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission checkPermission)
    {
        if (website != null && website.RolePermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator) || perms.WebsitePermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator))
            return true;

        if (Permissions.WebsitePermissions.HasFlag(checkPermission))
            return true;

        if (checkPermission == WebsitePermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission checkPermission)
    {
        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && (perms.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator) || perms.DockerPermissions.HasFlag(checkPermission)))
            return true;

        if (Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Permissions.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator))
            return true;

        if (Permissions.DockerPermissions.HasFlag(checkPermission))
            return true;

        if (Permissions.DockerPermissions.HasFlag(DockerPermission.DockerManager) && Perms.CheckDockerManagerPermission(checkPermission))
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

    public async Task DeleteAsync(TeamMemberData member, Action action)
    {
        FilterDefinition<TeamRoleData> filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Roles.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            _DB.Roles.Cache.TryRemove(Id, out _);
            Team.CachedRoles.Remove(Id);
            Team.TriggerRoleChange(this, false);
            _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.RoleDeleted)
                .SetTarget(Team)
                .AddProperty("Name", Name));

            var RoleSet = Team.Roles.Where(x => x != Id).ToHashSet();

            await Team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.Roles, RoleSet), () =>
            {
                Team.Roles = RoleSet;
                Team.TriggerRoleChange(this, false);

            });

            action?.Invoke();
        }
    }
}