﻿using DevSpaceWeb.Components.Layout;
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
}
