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
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public string GetUsername()
    {
        if (_DB.Users.TryGetValue(UserId, out PartialUserData? user))
            return user.UserName!;

        return "Unknown User";
    }

    public string GetCurrentName()
    {
        if (!string.IsNullOrEmpty(NickName))
            return NickName;

        if (_DB.Users.TryGetValue(UserId, out PartialUserData? user))
            return user.GetCurrentName();

        return "Unknown User";
    }

    public string GetCurrentColor()
    {
        if (Team != null)
        {
            foreach (ObjectId i in Roles.OrderByDescending(x => Team.RolePositions.GetValueOrDefault(x)))
            {
                if (_DB.Roles.Cache.TryGetValue(i, out TeamRoleData? role) && !string.IsNullOrEmpty(role.Color))
                    return role.Color;
            }

            if (!string.IsNullOrEmpty(Team.DefaultMembersColor))
                return Team.DefaultMembersColor;
        }
        return "#C4C9CE";
    }

    public string GetCurrentAvatar()
    {

        if (_DB.Users.TryGetValue(UserId, out PartialUserData? user))
        {
            if (AvatarId.HasValue && Team != null)
                return _Data.Config.Instance.GetPublicUrl() + "/public/files/" + Team.ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";


            return user.GetAvatarOrDefault(false);
        }

        return "https://cdn.fluxpoint.dev/devspace/user_avatar.webp";
    }

    public bool CanOwn(ITeamResource? resource)
    {
        if (resource == null)
            return false;

        if (TeamId != resource.TeamId)
            return false;

        TeamData? CurrentTeam = Team;
        if (CurrentTeam == null)
            return false;

        if (resource.OwnerId == UserId)
            return true;

        if (CurrentTeam.OwnerId == UserId)
            return true;

        if (HasTeamPermission(CurrentTeam, TeamPermission.GlobalAdministrator))
            return true;

        return false;
    }

    public bool CanManage(TeamMemberData? currentMember)
    {
        if (currentMember == null || TeamId != currentMember.TeamId)
            return false;

        TeamData? CurrentTeam = Team;
        if (CurrentTeam == null)
            return false;

        if (CurrentTeam.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > this.GetRank())
            return true;
        return false;
    }

    public UserDisabled? Disabled { get; set; }

    public bool IsGlobalAdministrator(TeamData? selectedTeam)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (HasTeamPermission(selectedTeam, TeamPermission.GlobalAdministrator))
            return true;

        return false;
    }

    public bool HasTeamPermission(TeamData? selectedTeam, TeamPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasTeamPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasTeamPermission(selectedTeam, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasAPIPermission(TeamData? selectedTeam, APIPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasAPIPermission(APIPermission.APIAdministrator))
            return true;

        if (selectedTeam.DefaultPermissions.HasAPIPermission(checkPermission))
            return true;

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasAPIPermission(selectedTeam, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasLogPermission(TeamData? selectedTeam, LogData? log, LogPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (log != null)
        {
            if (log.TeamId != TeamId || log.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasLogPermission(checkPermission))
            return true;

        if (log != null)
        {
            if (log.DefaultPermissions.HasLogPermission(checkPermission))
                return true;

            if (log.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? permOverride) && permOverride.HasLogPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasLogPermission(selectedTeam, log, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasProjectPermission(TeamData? selectedTeam, ProjectData? project, ProjectPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (project != null)
        {
            if (project.TeamId != TeamId || project.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasProjectPermission(checkPermission))
            return true;

        if (project != null)
        {
            if (project.DefaultPermissions.HasProjectPermission(checkPermission))
                return true;

            if (project.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasProjectPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasProjectPermission(selectedTeam, project, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasServerPermission(TeamData? selectedTeam, ServerData? server, ServerPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (server != null)
        {
            if (server.TeamId != TeamId || server.TeamId != selectedTeam.Id)
                return false;
        }

        if (TeamId != selectedTeam.Id)
            return false;

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasServerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasServerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasServerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasServerPermission(selectedTeam, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasWebsitePermission(TeamData? selectedTeam, WebsiteData? website, WebsitePermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (website != null)
        {
            if (website.TeamId != TeamId || website.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasWebsitePermission(checkPermission))
            return true;

        if (website != null)
        {
            if (website.DefaultPermissions.HasWebsitePermission(checkPermission))
                return true;

            if (website.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasWebsitePermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasWebsitePermission(selectedTeam, website, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasConsolePermission(TeamData? selectedTeam, ConsoleData? console, ConsolePermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (console != null)
        {
            if (console.TeamId != TeamId || console.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasConsolePermission(checkPermission))
            return true;

        if (console != null)
        {
            if (console.DefaultPermissions.HasConsolePermission(checkPermission))
                return true;

            if (console.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasConsolePermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasConsolePermission(selectedTeam, console, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasDockerPermission(TeamData? selectedTeam, ServerData? server, DockerPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (server != null)
        {
            if (server.TeamId != TeamId || server.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasDockerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasDockerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasDockerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerPermission(selectedTeam, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public bool HasDockerContainerPermission(TeamData? selectedTeam, ServerData? server, DockerContainerPermission checkPermission)
    {
        if (selectedTeam == null)
            return false;

        if (TeamId != selectedTeam.Id)
            return false;

        if (server != null)
        {
            if (server.TeamId != TeamId || server.TeamId != selectedTeam.Id)
                return false;
        }

        if (selectedTeam.OwnerId == UserId)
            return true;

        if (selectedTeam.DefaultPermissions.HasDockerContainerPermission(checkPermission))
            return true;

        if (server != null)
        {
            if (server.DefaultPermissions.HasDockerContainerPermission(checkPermission))
                return true;

            if (server.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? uovr) && uovr.HasDockerContainerPermission(checkPermission))
                return true;
        }

        foreach (ObjectId r in Roles)
        {
            if (selectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role))
            {
                if (role.HasDockerContainerPermission(selectedTeam, server, checkPermission))
                    return true;
            }
        }

        return false;
    }

    public int GetRank()
    {
        TeamData? SelectedTeam = Team;
        if (SelectedTeam == null)
            return -1;

        if (TeamId != SelectedTeam.Id)
            return -1;

        if (SelectedTeam.OwnerId == UserId)
            return int.MaxValue;

        int CurrentRank = -1;
        foreach (ObjectId r in Roles)
        {
            if (SelectedTeam.CachedRoles.TryGetValue(r, out TeamRoleData? role) && role.GetPosition() > CurrentRank)
            {
                CurrentRank = role.GetPosition();
            }
        }
        return CurrentRank;
    }

    public HashSet<ObjectId> Roles { get; set; } = new HashSet<ObjectId>();

    public IEnumerable<TeamRoleData> GetCachedRoles()
    {
        TeamData? selectedTeam = Team;
        if (selectedTeam == null)
            return new List<TeamRoleData>();
        return (IEnumerable<TeamRoleData>)Roles.Select(x => selectedTeam.CachedRoles.GetValueOrDefault(x)).Where(x => x != null);
    }
    public string? NickName { get; set; }

    public Guid? AvatarId { get; set; }

    public PermissionsSet CalculatePermissions(ITeamResource? resource)
    {
        PermissionsSet Permissions = new PermissionsSet();
        TeamData? selectedTeam = Team;
        if (selectedTeam == null)
            return new PermissionsSet();

        if (TeamId != selectedTeam.Id)
            return new PermissionsSet();

        if (UserId == selectedTeam.OwnerId || selectedTeam.DefaultPermissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
        {
            return PermissionsSet.MaxPermissions;
        }
        else
        {
            Permissions.AddFrom(selectedTeam.DefaultPermissions);

            if (resource != null && resource.MemberPermissionOverrides.TryGetValue(UserId, out PermissionsSet? perms))
            {
                if (perms.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                    return PermissionsSet.MaxPermissions;

                Permissions.AddFrom(perms);
            }

            foreach (ObjectId i in Roles)
            {
                if (selectedTeam.CachedRoles.TryGetValue(i, out TeamRoleData? role))
                {
                    if (role.Permissions.TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
                    {
                        return PermissionsSet.MaxPermissions;
                    }
                    else
                    {
                        Permissions.AddFrom(role.Permissions);

                        if (resource != null && resource.RolePermissionOverrides.TryGetValue(i, out PermissionsSet? roleOverride))
                        {
                            Permissions.AddFrom(roleOverride);
                        }
                    }
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

    [JsonIgnore]
    [BsonIgnore]
    public object RolesLock = new object();

    public void UpdateRoles(TeamMemberData member, HashSet<ObjectId> roles)
    {
        lock (RolesLock)
        {
            FilterDefinition<TeamMemberData> filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
            UpdateDefinition<TeamMemberData> update = new UpdateDefinitionBuilder<TeamMemberData>()
                .Set(x => x.Roles, roles);

            UpdateResult Result = _DB.Members.Collection.UpdateOne(filter, update);
            if (Result.IsAcknowledged)
            {
                Roles = roles;
                if (roles.Count != 0)
                {
                    _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.MemberRolesChanged)
                        .SetTarget(this)
                        .AddProperty("Roles", string.Join(", ", roles.Select(x => x.ToString()))));
                }
                else
                {
                    _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.MemberRolesChanged)
                        .SetTarget(this)
                        .AddProperty("Roles", null));
                }
            }
        }
    }
}
