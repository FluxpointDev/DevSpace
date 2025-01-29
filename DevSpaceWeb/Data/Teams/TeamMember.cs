using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
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

    public string? GetUserName()
    {
        if (_DB.Users.TryGetValue(UserId, out var user))
            return user.UserName;

        return null;
    }

    public bool CanManage(TeamMemberData currentMember)
    {
        if (Team.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > this.GetRank())
            return true;
        return false;
    }

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

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasTeamPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasTeamPermission(checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasAPIPermission(APIPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasAPIPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasAPIPermission(checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasLogPermission(LogData log, LogPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasLogPermission(checkPermission))
            return true;

        if (log.MemberPermissionOverrides.TryGetValue(log.Id, out PermissionsSet? permOverride) && permOverride.HasLogPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasLogPermission(log, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasProjectPermission(ProjectData project, ProjectPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasProjectPermission(checkPermission))
            return true;

        if (project.MemberPermissionOverrides.TryGetValue(project.Id, out PermissionsSet? uovr) && uovr.HasProjectPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasProjectPermission(project, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasServerPermission(ServerData server, ServerPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasServerPermission(checkPermission))
            return true;

        if (server != null && server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.HasServerPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasServerPermission(server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasWebsitePermission(WebsiteData website, WebsitePermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasWebsitePermission(checkPermission))
            return true;

        if (website.MemberPermissionOverrides.TryGetValue(website.Id, out PermissionsSet? uovr) && uovr.HasWebsitePermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasWebsitePermission(website, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasConsolePermission(ConsoleData console, ConsolePermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasConsolePermission(checkPermission))
            return true;

        if (console != null && console.MemberPermissionOverrides.TryGetValue(console.Id, out PermissionsSet? uovr) && uovr.HasConsolePermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasConsolePermission(console, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasDockerPermission(ServerData server, DockerPermission checkPermission)
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return false;

        if (SelectedTeam.OwnerId == UserId)
            return true;

        if (SelectedTeam.DefaultPermissions.HasDockerPermission(checkPermission))
            return true;

        if (server.MemberPermissionOverrides.TryGetValue(server.Id, out PermissionsSet? uovr) && uovr.HasDockerPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerPermission(server, checkPermission))
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
        PermissionsSet Permissions = null!;
        TeamData CurrentTeam = Team;
        if (CurrentTeam == null)
            return new PermissionsSet();

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
