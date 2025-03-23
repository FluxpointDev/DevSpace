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

    public int GetRank()
    {
        if (Team != null && Team.Members.TryGetValue(OwnerId, out ObjectId memberObj) && Team.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
            return member.GetRank();

        if (IsInstanceAdmin)
            return int.MaxValue;

        return 0;
    }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId.GetValueOrDefault());

    public bool HasAccess(TeamMemberData member)
    {
        if (TeamId != member.TeamId)
            return false;

        if (OwnerId == member.UserId && member.HasAPIPermission(Team, APIPermission.ViewOwnAPIs))
            return true;

        if (member.HasAPIPermission(Team, APIPermission.ViewAllAPIs))
            return true;
        return false;
    }

    public bool CanGenerate(TeamMemberData member)
    {
        if (TeamId != member.TeamId)
            return false;

        if (member.HasAPIPermission(Team, APIPermission.ManageOwnAPIs) && OwnerId == member.UserId)
            return true;

        if (member.UserId == Team.OwnerId)
            return true;

        return false;
    }

    public bool CanManage(TeamMemberData member)
    {
        if (TeamId != member.TeamId)
            return false;

        if (member.HasAPIPermission(Team, APIPermission.ManageOwnAPIs) && OwnerId == member.UserId)
            return true;

        if (member.HasAPIPermission(Team, APIPermission.APIAdministrator))
            return true;
        return false;
    }

    public bool HasTeamPermission(TeamData? SelectedTeam, TeamPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasTeamPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasTeamPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasTeamPermission(Team, checkPermission);
        }

        return false;
    }

    public bool HasAPIPermission(TeamData? SelectedTeam, APIPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasAPIPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasAPIPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasAPIPermission(Team, checkPermission);
        }

        return false;
    }

    public bool HasLogPermission(TeamData? SelectedTeam, LogData log, LogPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (log != null && log.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasLogPermission(checkPermission))
            return true;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasLogPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasLogPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasLogPermission(Team, log, checkPermission);
        }




        return false;
    }

    public bool HasProjectPermission(TeamData? SelectedTeam, ProjectData project, ProjectPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (project != null && project.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasProjectPermission(checkPermission))
            return true;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasProjectPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasProjectPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasProjectPermission(Team, project, checkPermission);
        }
        return false;
    }

    public bool HasServerPermission(TeamData? SelectedTeam, ServerData server, ServerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasServerPermission(checkPermission))
            return true;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasServerPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasServerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasServerPermission(Team, server, checkPermission);



        }
        return false;
    }

    public bool HasConsolePermission(TeamData? SelectedTeam, ConsoleData console, ConsolePermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (console != null && console.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasConsolePermission(checkPermission))
            return true;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasConsolePermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasConsolePermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasConsolePermission(Team, console, checkPermission);

        }




        return false;
    }

    public bool HasWebsitePermission(TeamData? SelectedTeam, WebsiteData website, WebsitePermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (website != null && website.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasWebsitePermission(checkPermission))
            return true;

        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasWebsitePermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasWebsitePermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasWebsitePermission(Team, website, checkPermission);
        }

        return false;
    }

    public bool HasDockerPermission(TeamData? SelectedTeam, ServerData server, DockerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasDockerPermission(checkPermission))
            return true;


        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasDockerPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasDockerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasDockerPermission(Team, server, checkPermission);
        }

        return false;
    }

    public bool HasDockerContainerPermission(TeamData? SelectedTeam, ServerData server, DockerContainerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet perms) && perms.HasDockerContainerPermission(checkPermission))
            return true;


        if (CustomPermissions != null)
        {
            if (CustomPermissions.HasDockerContainerPermission(checkPermission))
                return true;
        }
        else
        {
            if (SelectedTeam.DefaultPermissions.HasDockerContainerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData member))
                return member.HasDockerContainerPermission(Team, server, checkPermission);
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
