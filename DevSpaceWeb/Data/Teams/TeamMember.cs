using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamMemberData : IObject
{
    public ObjectId UserId { get; set; }
    public ObjectId TeamId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    [BsonIgnore]
    public string? FilterUsername => _DB.Users.TryGetValue(UserId, out PartialUserData? user) ? user.UserName : null;

    public string GetUsername()
    {
        if (_DB.Users.TryGetValue(UserId, out PartialUserData? user))
            return user.UserName!;

        return "Unknown User";
    }

    public string? GetCurrentName()
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
        return Static.DefaultRoleColor;
    }

    public string GetCurrentAvatar()
    {

        if (_DB.Users.TryGetValue(UserId, out PartialUserData? user))
        {
            if (AvatarId.HasValue && Team != null)
                return _Data.Config.Instance.GetPublicUrl() + "/public/files/" + Team.ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";


            return user.GetAvatarOrDefault(false);
        }

        return "https://cdn.fluxpoint.dev/devspace/default_avatar.webp";
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

        if (MemberPermissionExtensions.HasTeamPermission(this, CurrentTeam, TeamPermission.GlobalAdministrator))
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

    [BsonIgnore]
    public bool IsEnabled => Disabled == null;

    [BsonIgnore]
    public bool Has2FA => _DB.Users.TryGetValue(UserId, out PartialUserData? user) && user.Has2FA;

    [BsonIgnore]
    public string MemberType => GetMemberType();

    private string GetMemberType()
    {
        TeamData? GetTeam = Team;
        if (GetTeam == null)
            return "Member";

        if (GetTeam.OwnerId == UserId)
            return "Owner";

        if (MemberPermissionExtensions.HasTeamPermission(this, GetTeam, TeamPermission.GlobalAdministrator))
            return "Global Admin";

        if (MemberPermissionExtensions.HasTeamPermission(this, GetTeam, TeamPermission.TeamAdministrator))
            return "Team Admin";

        return "Member";
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

        if (MemberPermissionExtensions.HasTeamPermission(this, selectedTeam, TeamPermission.GlobalAdministrator))
            return true;

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

    public HashSet<ObjectId> Roles { get; set; } = [];

    public IEnumerable<TeamRoleData> GetCachedRoles()
    {
        TeamData? selectedTeam = Team;
        if (selectedTeam == null)
            return [];
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
