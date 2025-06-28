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

public class APIClient : IObject
{
    public required string Name { get; set; }
    public ObjectId OwnerId { get; set; }
    public ObjectId TeamId { get; set; }
    public bool IsDisabled { get; set; }
    [Obsolete("Removed for privacy")]
    public bool IsInstanceAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool UseCustomPermissions { get; set; }
    public ObjectId? GeneratedBy { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public PermissionsSet AllowedPermissions { get; set; } = new PermissionsSet();
    public PermissionsSet CustomPermissions { get; set; } = new PermissionsSet();
    public string? TokenHash { get; set; }

    public int GetRank()
    {
        if (Team != null && Team.Members.TryGetValue(OwnerId, out ObjectId memberObj) && Team.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
            return member.GetRank();

        return 0;
    }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public bool HasAccess(TeamMemberData member, bool checkAllowedPermissions)
    {
        if (Team == null)
            return false;

        if (TeamId != member.TeamId)
            return false;

        if (OwnerId == member.UserId && member.HasAPIPermission(Team, APIPermission.ViewOwnAPIs))
        {
            if (!checkAllowedPermissions || AllowedPermissions.HasAPIPermission(APIPermission.ViewOwnAPIs))
                return true;
        }

        if (member.HasAPIPermission(Team, APIPermission.ViewAllAPIs))
        {
            if (!checkAllowedPermissions || AllowedPermissions.HasAPIPermission(APIPermission.ViewAllAPIs))
                return true;
        }
        return false;
    }

    public bool CanGenerate(TeamMemberData member)
    {
        if (Team == null)
            return false;

        if (TeamId != member.TeamId)
            return false;

        if (member.HasAPIPermission(Team, APIPermission.ManageOwnAPIs) && OwnerId == member.UserId)
            return true;

        if (member.UserId == Team.OwnerId)
            return true;

        return false;
    }

    public bool CanManage(TeamMemberData member, bool checkAllowedPermissions)
    {
        if (Team == null)
            return false;

        if (TeamId != member.TeamId)
            return false;

        if (member.HasAPIPermission(Team, APIPermission.ManageOwnAPIs) && OwnerId == member.UserId)
        {
            if (!checkAllowedPermissions || AllowedPermissions.HasAPIPermission(APIPermission.ManageOwnAPIs))
                return true;
        }

        if (member.HasAPIPermission(Team, APIPermission.APIAdministrator))
        {
            if (!checkAllowedPermissions || AllowedPermissions.HasAPIPermission(APIPermission.APIAdministrator))
                return true;
        }
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
            if (!AllowedPermissions.HasTeamPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasTeamPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
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

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasAPIPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasAPIPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasAPIPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasAPIPermission(Team, checkPermission);
        }

        return false;
    }

    public bool HasLogPermission(TeamData? SelectedTeam, LogData? log, LogPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (log != null && log.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasLogPermission(checkPermission))
            return true;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasLogPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasLogPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasLogPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasLogPermission(Team, log, checkPermission);
        }




        return false;
    }

    public bool HasProjectPermission(TeamData? SelectedTeam, ProjectData? project, ProjectPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (project != null && project.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasProjectPermission(checkPermission))
            return true;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasProjectPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasProjectPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasProjectPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasProjectPermission(Team, project, checkPermission);
        }
        return false;
    }

    public bool HasServerPermission(TeamData? SelectedTeam, ServerData? server, ServerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasServerPermission(checkPermission))
            return true;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasServerPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasServerPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasServerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasServerPermission(Team, server, checkPermission);



        }
        return false;
    }

    public bool HasConsolePermission(TeamData? SelectedTeam, ConsoleData? console, ConsolePermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (console != null && console.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasConsolePermission(checkPermission))
            return true;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasConsolePermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasConsolePermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasConsolePermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasConsolePermission(Team, console, checkPermission);

        }




        return false;
    }

    public bool HasWebsitePermission(TeamData? SelectedTeam, WebsiteData? website, WebsitePermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (website != null && website.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasWebsitePermission(checkPermission))
            return true;

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasWebsitePermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasWebsitePermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasWebsitePermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasWebsitePermission(Team, website, checkPermission);
        }

        return false;
    }

    public bool HasDockerPermission(TeamData? SelectedTeam, ServerData? server, DockerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasDockerPermission(checkPermission))
            return true;


        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasDockerPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasDockerPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasDockerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasDockerPermission(Team, server, checkPermission);
        }

        return false;
    }

    public bool HasDockerContainerPermission(TeamData? SelectedTeam, ServerData? server, DockerContainerPermission checkPermission)
    {
        if (SelectedTeam == null)
            return false;

        if (TeamId != SelectedTeam.Id)
            return false;

        if (server != null && server.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? perms) && perms.HasDockerContainerPermission(checkPermission))
            return true;


        if (UseCustomPermissions)
        {
            if (CustomPermissions != null && CustomPermissions.HasDockerContainerPermission(checkPermission))
                return true;
        }
        else
        {
            if (!AllowedPermissions.HasDockerContainerPermission(checkPermission))
                return false;

            if (SelectedTeam.DefaultPermissions.HasDockerContainerPermission(checkPermission))
                return true;

            if (SelectedTeam.Members.TryGetValue(OwnerId, out ObjectId memberObj) && SelectedTeam.CachedMembers.TryGetValue(memberObj, out TeamMemberData? member))
                return member.HasDockerContainerPermission(Team, server, checkPermission);
        }

        return false;
    }

    public PermissionsSet CalculatePermissions(ITeamResource? resource)
    {
        PermissionsSet Permissions = new PermissionsSet();
        TeamData? CurrentTeam = Team;
        if (CurrentTeam == null)
            return new PermissionsSet();

        if (TeamId != CurrentTeam.Id)
            return new PermissionsSet();

        if (resource != null && resource.ApiPermissionOverrides.TryGetValue(Id, out PermissionsSet? overrideSet))
        {
            if (overrideSet.TeamPermissions == TeamPermission.GlobalAdministrator)
                return PermissionsSet.MaxPermissions;

            Permissions.AddFrom(overrideSet);
        }

        if (UseCustomPermissions)
        {
            if (CustomPermissions != null)
                Permissions.AddFrom(CustomPermissions);
        }
        else
        {
            foreach (TeamPermission i in Enum.GetValues<TeamPermission>())
            {
                if (HasTeamPermission(Team, i))
                    Permissions.TeamPermissions |= i;
            }

            foreach (APIPermission i in Enum.GetValues<APIPermission>())
            {
                if (HasAPIPermission(Team, i))
                    Permissions.APIPermissions |= i;
            }

            foreach (LogPermission i in Enum.GetValues<LogPermission>())
            {
                if (HasLogPermission(Team, resource as LogData, i))
                    Permissions.LogPermissions |= i;
            }

            foreach (ProjectPermission i in Enum.GetValues<ProjectPermission>())
            {
                if (HasProjectPermission(Team, resource as ProjectData, i))
                    Permissions.ProjectPermissions |= i;
            }

            foreach (ServerPermission i in Enum.GetValues<ServerPermission>())
            {
                if (HasServerPermission(Team, resource as ServerData, i))
                    Permissions.ServerPermissions |= i;
            }

            foreach (ConsolePermission i in Enum.GetValues<ConsolePermission>())
            {
                if (HasConsolePermission(Team, resource as ConsoleData, i))
                    Permissions.ConsolePermissions |= i;
            }

            foreach (WebsitePermission i in Enum.GetValues<WebsitePermission>())
            {
                if (HasWebsitePermission(Team, resource as WebsiteData, i))
                    Permissions.WebsitePermissions |= i;
            }

            foreach (DockerPermission i in Enum.GetValues<DockerPermission>())
            {
                if (HasDockerPermission(Team, resource as ServerData, i))
                    Permissions.DockerPermissions |= i;
            }

            foreach (DockerContainerPermission i in Enum.GetValues<DockerContainerPermission>())
            {
                if (HasDockerContainerPermission(Team, resource as ServerData, i))
                    Permissions.DockerContainerPermissions |= i;
            }
        }
        return Permissions;
    }


    public async Task<UpdateResult> UpdateAsync(UpdateDefinition<APIClient> update, Action? action = null)
    {
        FilterDefinition<APIClient> filter = Builders<APIClient>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.API.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();

        return Result;
    }

    public async Task<DeleteResult> DeleteAsync(Action? action = null)
    {
        FilterDefinition<APIClient> filter = Builders<APIClient>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.API.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            _DB.API.Cache.TryRemove(Id, out _);
            action?.Invoke();
        }

        return Result;
    }
}
