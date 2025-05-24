using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamData : IResource
{
    public string? VanityUrl { get; set; }

    public Guid? ResourceId { get; set; }

    public Guid? IconId { get; set; }

    [BsonIgnore]
    public FileResource Icon => new FileResource("Icon", ResourceId, IconId);

    public event EventHandler<Tuple<TeamRoleData?, bool>> RoleChangeEvent;
    public void TriggerRoleChange(TeamRoleData? role, bool isAdd)
    {
        RoleChangeEvent?.Invoke(this, new Tuple<TeamRoleData?, bool>(role, isAdd));
    }

    [BsonIgnore]
    public bool HasIcon => IconId != null;

    public string GetIconOrDefault(bool usePng = false)
    {
        if (!HasIcon)
            return "https://cdn.fluxpoint.dev/devspace/default_avatar." + (usePng ? "png" : "webp");

        return Icon.Url(usePng ? "png" : "webp");
    }

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();
    public string? DefaultMembersColor { get; set; }
    public bool Require2FA { get; set; }

    [Obsolete("Use cached roles")]
    [BsonIgnoreIfNull]
    public HashSet<ObjectId>? Roles = null;

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, ObjectId> Members = [];

    public TeamMemberData? GetMember(PartialUserData user)
    {
        if (Members.TryGetValue(user.Id, out ObjectId memberObj))
            return CachedMembers[memberObj];

        return null;
    }

    public TeamMemberData? GetMember(AuthUser user)
    {
        if (Members.TryGetValue(user.Id, out ObjectId memberObj))
            return CachedMembers[memberObj];

        return null;
    }

    public TeamMemberData? GetMember(SessionProvider session)
    {
        if (session.CurrentUser != null)
            return CachedMembers.GetValueOrDefault(Members.GetValueOrDefault(session.CurrentUser.Id));
        return null;
    }

    [JsonIgnore]
    [BsonIgnore]
    public Dictionary<ObjectId, TeamRoleData> CachedRoles = [];

    [JsonIgnore]
    [BsonIgnore]
    public Dictionary<ObjectId, TeamMemberData> CachedMembers = [];

    public string GetVanityUrlOrId()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public VanityUrlData GetVanityUrlData()
    {
        if (_DB.TeamVanityUrls.Cache.TryGetValue(Id, out VanityUrlData? data))
            return data;
        data = new VanityUrlData
        {
            Id = Id
        };
        _ = _DB.TeamVanityUrls.CreateAsync(data);
        return data;
    }

    public async Task<UpdateResult> UpdateAsync(UpdateDefinition<TeamData> update, Action? action = null)
    {
        FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Teams.Collection.UpdateOneAsync(filter, update);
        if (action != null && Result.IsAcknowledged)
            action?.Invoke();
        return Result;
    }

    [JsonIgnore]
    [BsonIgnore]
    public object RolePositionLock = new object();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, int> RolePositions = [];

    public bool AddRole(TeamMemberData member, TeamRoleData role)
    {
        lock (RolePositionLock)
        {
            Dictionary<ObjectId, int> UpdatedPositions = [];
            int Position = 1;
            foreach (KeyValuePair<ObjectId, int> i in RolePositions.OrderBy(x => x.Value))
            {
                Logger.LogMessage($"Set Position: {i.Key} - {Position}", LogSeverity.Debug);
                UpdatedPositions.Add(i.Key, Position);
                Position += 1;
            }
            UpdatedPositions.Add(role.Id, 0);


            FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
            UpdateDefinition<TeamData> update = new UpdateDefinitionBuilder<TeamData>()
                .Set(x => x.RolePositions, UpdatedPositions);

            UpdateResult Result = _DB.Teams.Collection.UpdateOne(filter, update);
            if (Result.IsAcknowledged)
            {
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.RoleCreated)
                    .SetTarget(role)
                    .AddProperty("Description", role.Description));

                _DB.Roles.Cache.TryAdd(role.Id, role);
                CachedRoles.TryAdd(role.Id, role);
                RolePositions = UpdatedPositions;
                this.TriggerRoleChange(role, true);
            }
            else
                return false;
        }

        return true;
    }

    public bool DeleteRole(TeamMemberData member, TeamRoleData role)
    {
        lock (RolePositionLock)
        {
            Dictionary<ObjectId, int> UpdatedPositions = [];
            int Position = 0;
            foreach (TeamRoleData? i in CachedRoles.Values.OrderBy(x => x.GetPosition()))
            {
                if (i.Id == role.Id)
                    continue;

                Logger.LogMessage($"Set Position: {i.Name} - {Position}", LogSeverity.Debug);
                UpdatedPositions.Add(i.Id, Position);
                Position += 1;
            }
            FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
            UpdateDefinition<TeamData> update = new UpdateDefinitionBuilder<TeamData>()
               .Set(x => x.RolePositions, UpdatedPositions);

            UpdateResult Result = _DB.Teams.Collection.UpdateOne(filter, update);
            if (Result.IsAcknowledged)
            {
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.RoleDeleted)
                .SetTarget(role));

                CachedRoles.Remove(role.Id);
                RolePositions = UpdatedPositions;
                _DB.Roles.Cache.TryRemove(role.Id, out _);
                this.TriggerRoleChange(role, false);
            }
            else
                return false;
        }

        return true;
    }

    public void UpdateRolesPosition(TeamMemberData member, TeamRoleData role, bool moveUp)
    {
        lock (RolePositionLock)
        {
            if (!member.HasTeamPermission(this, TeamPermission.ManageRoles))
                return;

            int CurrentPosition = role.GetPosition();
            int CurrentRank = member.GetRank();
            if (CurrentRank <= CurrentPosition)
                return;

            if (moveUp)
            {
                if (CurrentRank <= (CurrentPosition + 1))
                    return;
            }


            int Position = 0;
            Dictionary<ObjectId, int> UpdatedPositions = [];
            ObjectId? LastRoleId = null;
            bool CheckLastRole = true;
            ObjectId? NextRoleId = null;
            bool CheckNextRole = false;

            if (member.Team == null)
                return;

            foreach (TeamRoleData? i in member.Team.CachedRoles.Values.OrderBy(x => x.GetPosition()))
            {
                // Check current next role to move down
                if (i.Id != role.Id && CheckNextRole && !NextRoleId.HasValue)
                    NextRoleId = i.Id;

                // Finish checking last role and move to next role
                if (i.Id == role.Id)
                {
                    CheckLastRole = false;
                    CheckNextRole = true;
                }

                // Check current last role to move down
                if (CheckLastRole)
                    LastRoleId = i.Id;


                UpdatedPositions.Add(i.Id, Position);
                Position += 1;
            }

            if (moveUp)
            {
                if (NextRoleId.HasValue)
                {
                    UpdatedPositions[NextRoleId.Value] = UpdatedPositions[NextRoleId.Value] - 1;
                    UpdatedPositions[role.Id] = UpdatedPositions[role.Id] + 1;
                }
            }
            else
            {
                if (LastRoleId.HasValue)
                {
                    UpdatedPositions[LastRoleId.Value] = UpdatedPositions[LastRoleId.Value] + 1;
                    UpdatedPositions[role.Id] = UpdatedPositions[role.Id] - 1;
                }
            }

            FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
            UpdateDefinition<TeamData> update = new UpdateDefinitionBuilder<TeamData>()
               .Set(x => x.RolePositions, UpdatedPositions);

            UpdateResult Result = _DB.Teams.Collection.UpdateOne(filter, update);
            if (Result.IsAcknowledged)
            {
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Role, AuditLogEventType.RolePositionChanged)
                        .SetTarget(role)
                        .AddProperty("Position", UpdatedPositions[role.Id].ToString())
                        .AddProperty("Moved", moveUp ? "Up" : "Down"));

                RolePositions = UpdatedPositions;
                this.TriggerRoleChange(null, false);
            }
        }
    }
}
