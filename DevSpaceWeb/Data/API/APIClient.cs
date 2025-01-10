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

    public bool HasTeamPermission(TeamPermission permission)
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

            if (CustomPermissions.TeamPermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator)
                return true;

            if (Perms.Team.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasTeamPermission(permission);
        }

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission permission)
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

            if (CustomPermissions.LogPermissions.HasFlag(LogPermission.LogAdministrator) || CustomPermissions.LogPermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Log.LogAdministrator)
                return true;

            if (Perms.Log.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasLogPermission(log, permission);
        }

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out PermissionsSet? uovr) && uovr.LogPermissions.HasFlag(permission))
            return true;


        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission permission)
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

            if (CustomPermissions.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator) || CustomPermissions.ProjectPermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Project.ProjectAdministrator)
                return true;

            if (Perms.Project.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasProjectPermission(project, permission);
        }

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out PermissionsSet? uovr) && uovr.ProjectPermissions.HasFlag(permission))
            return true;


        return false;
    }

    public bool HasServerPermission(ServerData? server, ServerPermission permission)
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

            if (CustomPermissions.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || CustomPermissions.ServerPermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Server.ServerAdministrator)
                return true;

            if (permission == ServerPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
                return true;

            if (Perms.Server.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasServerPermission(server, permission);
        }

        if (server != null && server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.ServerPermissions.HasFlag(permission))
            return true;


        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission permission)
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

            if (CustomPermissions.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator) || CustomPermissions.WebsitePermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Website.WebsiteAdministrator)
                return true;

            if (Perms.Website.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasWebsitePermission(website, permission);
        }

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out PermissionsSet? uovr) && uovr.WebsitePermissions.HasFlag(permission))
            return true;


        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission permission)
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

            if (CustomPermissions.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator) || CustomPermissions.DockerPermissions.HasFlag(permission))
                return true;
        }
        else
        {
            PermissionsAll Perms = SelectedTeam.GetPermissions();

            if (Perms.Team.GlobalAdministrator || Perms.Docker.DockerAdministrator)
                return true;

            if (Perms.Docker.Has(permission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasDockerPermission(server, permission);
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
}
