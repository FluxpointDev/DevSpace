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
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return Icon.Url(usePng ? "png" : "webp");
    }

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();


    public HashSet<ObjectId> Roles = new HashSet<ObjectId>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, ObjectId> Members = new Dictionary<ObjectId, ObjectId>();

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
    public Dictionary<ObjectId, TeamRoleData> CachedRoles = new Dictionary<ObjectId, TeamRoleData>();

    [JsonIgnore]
    [BsonIgnore]
    public Dictionary<ObjectId, TeamMemberData> CachedMembers = new Dictionary<ObjectId, TeamMemberData>();

    public string GetVanityUrlOrId()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public VanityUrlData GetVanityUrlData()
    {
        if (_DB.TeamVanityUrls.Cache.TryGetValue(Id, out VanityUrlData data))
            return data;
        data = new VanityUrlData
        {
            Id = Id
        };
        _DB.TeamVanityUrls.CreateAsync(data);
        return data;
    }

    public async Task UpdateAsync(UpdateDefinition<TeamData> update, Action action)
    {
        FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Teams.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    [JsonIgnore]
    [BsonIgnore]
    public object RolePositionLock = new object();

    public void UpdateRolesPosition(TeamMemberData member, TeamRoleData role, bool moveUp)
    {
        lock (RolePositionLock)
        {
            if (!member.HasTeamPermission(TeamPermission.ManageRoles))
                return;

            int CurrentRank = member.GetRank();
            if (CurrentRank <= role.Position)
                return;

            int Position = 0;
            bool IsOutOfDate = false;
            TeamRoleData? MoveRole = null;
            foreach (var i in _DB.Roles.Cache.Values.Where(x => x.TeamId == member.TeamId).OrderBy(x => x.Position))
            {
                if (moveUp)
                {
                    if (i.Position == (role.Position + 1))
                        MoveRole = i;
                }
                else
                {
                    if (i.Position == (role.Position - 1))
                        MoveRole = i;
                }

                if (i.Position != Position)
                {
                    IsOutOfDate = true;
                    break;
                }
                Position += 1;
            }

            List<WriteModel<TeamRoleData>> list = new List<WriteModel<TeamRoleData>>();
            List<TeamRoleData> Roles = new List<TeamRoleData>();
            if (IsOutOfDate)
            {
                int CurrentIndex = -1;
                foreach (var i in _DB.Roles.Cache.Values.Where(x => x.TeamId == member.TeamId).OrderBy(x => x.Position))
                {
                    if (role.Id == i.Id)
                        CurrentIndex = Roles.Count();
                    Roles.Add(i);
                }
                if (CurrentIndex != -1)
                {
                    if (moveUp)
                    {
                        if (CurrentIndex < Roles.Count())
                        {
                            Roles.RemoveAt(CurrentIndex);
                            Roles.Insert(CurrentIndex + 1, role);
                        }
                    }
                    else
                    {
                        if (CurrentIndex != 0)
                        {
                            Roles.RemoveAt(CurrentIndex);
                            Roles.Insert(CurrentIndex - 1, role);
                        }
                    }
                }
                Position = 0;
                foreach (var i in Roles)
                {
                    FilterDefinition<TeamRoleData> filter = new FilterDefinitionBuilder<TeamRoleData>().Eq(x => x.Id, i.Id);
                    UpdateDefinition<TeamRoleData> update = new UpdateDefinitionBuilder<TeamRoleData>().Set(x => x.Position, Position);
                    list.Add(new UpdateOneModel<TeamRoleData>(filter, update));
                    Position += 1;
                }
            }
            else
            {
                if (MoveRole == null)
                    return;

                FilterDefinition<TeamRoleData> firstFilter = new FilterDefinitionBuilder<TeamRoleData>().Eq(x => x.Id, role.Id);
                UpdateDefinition<TeamRoleData> firstUpdate = new UpdateDefinitionBuilder<TeamRoleData>().Set(x => x.Position, moveUp ? role.Position + 1 : role.Position - 1);
                list.Add(new UpdateOneModel<TeamRoleData>(firstFilter, firstUpdate));

                FilterDefinition<TeamRoleData> secondFilter = new FilterDefinitionBuilder<TeamRoleData>().Eq(x => x.Id, MoveRole.Id);
                UpdateDefinition<TeamRoleData> secondUpdate = new UpdateDefinitionBuilder<TeamRoleData>().Set(x => x.Position, moveUp ? MoveRole.Position - 1 : MoveRole.Position + 1);
                list.Add(new UpdateOneModel<TeamRoleData>(secondFilter, secondUpdate));
            }


            BulkWriteResult<TeamRoleData> result = _DB.Roles.Collection.BulkWrite(list);
            if (result.IsAcknowledged)
            {
                if (IsOutOfDate)
                {
                    Position = 0;
                    foreach (var i in Roles)
                    {
                        i.Position = Position;
                        Position += 1;
                    }
                }
                else
                {
                    if (moveUp)
                    {
                        role.Position += 1;
                        MoveRole.Position -= 1;
                    }
                    else
                    {
                        role.Position -= 1;
                        MoveRole.Position += 1;
                    }

                }
                TriggerRoleChange(null, true);
            }

        }
    }
}
