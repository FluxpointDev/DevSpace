using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.API;

public class APIClient
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId OwnerId { get; set; }
    public ObjectId? TeamId { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsInstanceAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool UseCustomPermissions { get; set; }
    public ObjectId? GeneratedBy { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public PermissionsSet CustomPermissions { get; set; } = new PermissionsSet();
    public string? TokenHash { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId.GetValueOrDefault());

    public bool HasAccess(TeamMemberData member)
    {
        if (OwnerId == member.UserId || member.HasTeamPermission(TeamPermission.ManageAllAPIs))
            return true;
        return false;
    }

    public bool CanManage(TeamMemberData member)
    {
        if ((member.HasTeamPermission(TeamPermission.ManageOwnAPIs) && OwnerId == member.UserId) || member.HasTeamPermission(TeamPermission.ManageAllAPIs))
            return true;
        return false;
    }

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.TeamPermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator)
                return true;

            if (Perms.Team.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasTeamPermission(checkPermission);
        }

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.LogPermissions.HasFlag(LogPermission.LogAdministrator) || CustomPermissions.LogPermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Log.LogAdministrator)
                return true;

            if (Perms.Log.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasLogPermission(log, checkPermission);
        }

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out PermissionsSet? uovr) && uovr.LogPermissions.HasFlag(checkPermission))
            return true;


        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator) || CustomPermissions.ProjectPermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Project.ProjectAdministrator)
                return true;

            if (Perms.Project.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasProjectPermission(project, checkPermission);
        }

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out PermissionsSet? uovr) && uovr.ProjectPermissions.HasFlag(checkPermission))
            return true;


        return false;
    }

    public bool HasServerPermission(ServerData? server, ServerPermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || CustomPermissions.ServerPermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Server.ServerAdministrator)
                return true;

            if (checkPermission == ServerPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
                return true;

            if (Perms.Server.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasServerPermission(server, checkPermission);
        }

        if (server != null && server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.ServerPermissions.HasFlag(checkPermission))
            return true;


        return false;
    }

    public bool HasConsolePermission(ConsoleData? console, ConsolePermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator) || CustomPermissions.ConsolePermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Console.ConsoleAdministrator)
                return true;

            if (checkPermission == ConsolePermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
                return true;

            if (Perms.Console.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
            {
                Console.WriteLine("Check Owner: " + member.Id);
                return member.HasConsolePermission(console, checkPermission);
            }
        }

        if (console != null && console.MemberPermissionOverrides.TryGetValue(console.Id, out PermissionsSet? uovr) && uovr.ConsolePermissions.HasFlag(checkPermission))
            return true;


        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator) || CustomPermissions.WebsitePermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Website.WebsiteAdministrator)
                return true;

            if (Perms.Website.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasWebsitePermission(website, checkPermission);
        }

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out PermissionsSet? uovr) && uovr.WebsitePermissions.HasFlag(checkPermission))
            return true;


        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission checkPermission)
    {
        if (IsInstanceAdmin)
            return true;

        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                return true;

            if (CustomPermissions.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator) || CustomPermissions.DockerPermissions.HasFlag(checkPermission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Docker.DockerAdministrator)
                return true;

            if (Perms.Docker.Has(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasDockerPermission(server, checkPermission);
        }

        return false;
    }

    public async Task UpdateAsync(UpdateDefinition<APIClient> update, Action action)
    {
        FilterDefinition<APIClient> filter = Builders<APIClient>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.API.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();

    }

    public async Task DeleteAsync(TeamMemberData member, Action action)
    {
        FilterDefinition<APIClient> filter = Builders<APIClient>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.API.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.APIClientDeleted)
                .SetTarget(Team)
                .AddProperty("Name", Name));

            _DB.API.Cache.TryRemove(Id, out _);
            action?.Invoke();
        }
    }
}
