using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
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

    public bool IsGlobalAdministrator()
    {
        TeamData CurrentTeam = Team;
        if (CurrentTeam.OwnerId == UserId)
            return true;
        if (CurrentTeam.DefaultPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        var Permissions = new PermissionsSet
        {
            TeamPermissions = CurrentTeam.DefaultPermissions.TeamPermissions,
        };

        foreach (var i in Roles)
        {
            if (CurrentTeam.CachedRoles.TryGetValue(i, out var role))
            {
                Permissions.TeamPermissions |= role.Permissions.TeamPermissions;
            }
        }

        return Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator);
    }

    public bool HasTeamPermission(TeamPermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (Default.TeamPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasTeamPermission(permission))
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

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.LogPermissions.HasFlag(LogPermission.LogAdministrator))
            return true;

        if (Default.LogPermissions.HasFlag(permission))
            return true;

        if (permission == LogPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out PermissionsSet? uovr) && uovr.LogPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasLogPermission(log, permission))
                    return true;
            }
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

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator))
            return true;

        if (Default.ProjectPermissions.HasFlag(permission))
            return true;

        if (permission == ProjectPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out PermissionsSet? uovr) && uovr.ProjectPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasProjectPermission(project, permission))
                    return true;
            }
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

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
            return true;

        if (Default.ServerPermissions.HasFlag(permission))
            return true;

        if (permission == ServerPermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (server != null && server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.ServerPermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasServerPermission(server, permission))
                    return true;
            }
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

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator))
            return true;

        if (Default.WebsitePermissions.HasFlag(permission))
            return true;

        if (permission == WebsitePermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out PermissionsSet? uovr) && uovr.WebsitePermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasWebsitePermission(website, permission))
                    return true;
            }
        }

        return false;
    }

    public bool HasConsolePermission(ConsoleData console, ConsolePermission permission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator))
            return true;

        if (Default.ConsolePermissions.HasFlag(permission))
            return true;

        if (permission == ConsolePermission.ManageResource && HasTeamPermission(TeamPermission.ManageResources))
            return true;

        if (console != null && console.MemberPermissionOverrides.TryGetValue(console.Id, out PermissionsSet? uovr) && uovr.ConsolePermissions.HasFlag(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasConsolePermission(console, permission))
                    return true;
            }
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

        PermissionsSet Default = SelectedTeam.DefaultPermissions;

        if (Default.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator) || Default.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator))
            return true;

        if (Default.DockerPermissions.HasFlag(permission))
            return true;

        if (server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.DockerPermissions.HasFlag(permission))
            return true;

        if (Default.DockerPermissions.HasFlag(DockerPermission.DockerManager) && Perms.CheckDockerManagerPermission(permission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerPermission(server, permission))
                    return true;
            }
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

    public IEnumerable<TeamRoleData> GetCachedRoles()
    {
        TeamData GetTeam = Team;
        return (IEnumerable<TeamRoleData>)Roles.Select(x => GetTeam.CachedRoles.GetValueOrDefault(x)).Where(x => x != null);
    }
    public string? NickName { get; set; }

    public Guid? AvatarId { get; set; }

    public PermissionsSet CalculatePermissions()
    {
        PermissionsSet Permissions = null;
        TeamData CurrentTeam = Team;
        if (UserId == CurrentTeam.OwnerId || CurrentTeam.DefaultPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
        {
            Permissions = new PermissionsSet
            {
                ConsolePermissions = (ConsolePermission)ulong.MaxValue,
                DockerPermissions = (DockerPermission)ulong.MaxValue,
                LogPermissions = (LogPermission)ulong.MaxValue,
                ProjectPermissions = (ProjectPermission)ulong.MaxValue,
                ServerPermissions = (ServerPermission)ulong.MaxValue,
                TeamPermissions = (TeamPermission)ulong.MaxValue,
                WebsitePermissions = (WebsitePermission)ulong.MaxValue
            };
        }
        else
        {
            Permissions = new PermissionsSet
            {
                ConsolePermissions = CurrentTeam.DefaultPermissions.ConsolePermissions,
                DockerPermissions = CurrentTeam.DefaultPermissions.DockerPermissions,
                LogPermissions = CurrentTeam.DefaultPermissions.LogPermissions,
                ProjectPermissions = CurrentTeam.DefaultPermissions.ProjectPermissions,
                ServerPermissions = CurrentTeam.DefaultPermissions.ServerPermissions,
                TeamPermissions = CurrentTeam.DefaultPermissions.TeamPermissions,
                WebsitePermissions = CurrentTeam.DefaultPermissions.WebsitePermissions
            };

            foreach (var i in Roles)
            {
                if (CurrentTeam.CachedRoles.TryGetValue(i, out var role))
                {
                    Permissions.ConsolePermissions |= role.Permissions.ConsolePermissions;
                    Permissions.DockerPermissions |= role.Permissions.DockerPermissions;
                    Permissions.LogPermissions |= role.Permissions.LogPermissions;
                    Permissions.ProjectPermissions |= role.Permissions.ProjectPermissions;
                    Permissions.ServerPermissions |= role.Permissions.ServerPermissions;
                    Permissions.TeamPermissions |= role.Permissions.TeamPermissions;
                    Permissions.WebsitePermissions |= role.Permissions.WebsitePermissions;
                }
            }
        }


        return Permissions;
    }

    public async Task UpdateAsync(UpdateDefinition<TeamMemberData> update, Action action)
    {
        FilterDefinition<TeamMemberData> filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Members.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
