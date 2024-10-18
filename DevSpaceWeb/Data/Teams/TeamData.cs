using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string VanityUrl { get; set; }
    public ObjectId OwnerId { get; set; }

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();

    [BsonIgnore]
    [JsonIgnore]
    private PermissionsAll _DefaultPermissions => new PermissionsAll(DefaultPermissions);

    public bool HasTeamPermission(TeamMemberData member, TeamPermission permission)
    {
        if (member.Team == null)
            return false;
        if (OwnerId == member.UserId)
            return true;

        if (member.Team._DefaultPermissions.Team.GlobalAdministrator || member.Team._DefaultPermissions.Team.TeamAdministrator)
            return true;

        if (member.Team._DefaultPermissions.Team.Has(permission))
            return true;

        foreach(var r in member.Roles)
        {
            if (CachedRoles.TryGetValue(r, out var role) && role.HasTeamPermission(member, permission))
                return true;
        }

        return false;
    }

    public bool HasLogPermission(TeamMemberData member, LogPermission permission)
    {
        if (member.Team == null)
            return false;

        if (OwnerId == member.UserId)
            return true;

        if (member.Team._DefaultPermissions.Team.GlobalAdministrator || member.Team._DefaultPermissions.Log.LogAdministrator)
            return true;

        if (member.Team._DefaultPermissions.Log.Has(permission))
            return true;

        foreach (var r in member.Roles)
        {
            if (CachedRoles.TryGetValue(r, out var role) && role.HasLogPermission(member, permission))
                return true;
        }

        return false;
    }

    public bool HasProjectPermission(TeamMemberData member, ProjectPermission permission)
    {
        if (member.Team == null)
            return false;

        if (OwnerId == member.UserId)
            return true;

        if (member.Team._DefaultPermissions.Team.GlobalAdministrator || member.Team._DefaultPermissions.Project.ProjectAdministrator)
            return true;

        if (member.Team._DefaultPermissions.Project.Has(permission))
            return true;

        foreach (var r in member.Roles)
        {
            if (CachedRoles.TryGetValue(r, out var role) && role.HasProjectPermission(member, permission))
                return true;
        }

        return false;
    }

    public bool HasServerPermission(TeamMemberData member, ServerPermission permission)
    {
        if (member.Team == null)
            return false;

        if (OwnerId == member.UserId)
            return true;

        if (member.Team._DefaultPermissions.Team.GlobalAdministrator || member.Team._DefaultPermissions.Server.ServerAdministrator)
            return true;

        if (member.Team._DefaultPermissions.Server.Has(permission))
            return true;

        foreach (var r in member.Roles)
        {
            if (CachedRoles.TryGetValue(r, out var role) && role.HasServerPermission(member, permission))
                return true;
        }

        return false;
    }

    public bool HasWebsitePermission(TeamMemberData member, WebsitePermission permission)
    {
        if (member.Team == null)
            return false;

        if (OwnerId == member.UserId)
            return true;

        if (member.Team._DefaultPermissions.Team.GlobalAdministrator || member.Team._DefaultPermissions.Website.WebsiteAdministrator)
            return true;

        if (member.Team._DefaultPermissions.Website.Has(permission))
            return true;

        foreach (var r in member.Roles)
        {
            if (CachedRoles.TryGetValue(r, out var role) && role.HasWebsitePermission(member, permission))
                return true;
        }

        return false;
    }

    public HashSet<ObjectId> Roles = new HashSet<ObjectId>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, ObjectId> Members = new Dictionary<ObjectId, ObjectId>();

    public TeamMemberData GetMember(PartialUserData user)
    {
        return CachedMembers[Members[user.Id]];
    }

    public TeamMemberData? GetMember(SessionProvider session)
    {
        if (session.CurrentUserId.HasValue)
            return CachedMembers.GetValueOrDefault(Members.GetValueOrDefault(session.CurrentUserId.Value));
        return null;
    }

    [JsonIgnore]
    [BsonIgnore]
    public Dictionary<ObjectId, TeamRoleData> CachedRoles = new Dictionary<ObjectId, TeamRoleData>();

    [JsonIgnore]
    [BsonIgnore]
    public Dictionary<ObjectId, TeamMemberData> CachedMembers = new Dictionary<ObjectId, TeamMemberData>();

    public string GetVanityUrl()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public void Update(UpdateDefinition<TeamData> update)
    {
        var filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
        _DB.Teams.Collection.UpdateOne(filter, update);
    }
}
public class TeamRoleData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId TeamId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PermissionsSet Permissions { get; set; } = new PermissionsSet();

    [JsonIgnore]
    [BsonIgnore]
    private PermissionsAll _Permissions => new PermissionsAll(Permissions);

    public bool HasTeamPermission(TeamMemberData member, TeamPermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Team.TeamAdministrator)
            return true;

        return _Permissions.Team.Has(permission);
    }

    public bool HasLogPermission(TeamMemberData member, LogPermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Log.LogAdministrator)
            return true;

        return _Permissions.Log.Has(permission);
    }

    public bool HasProjectPermission(TeamMemberData member, ProjectPermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Project.ProjectAdministrator)
            return true;

        return _Permissions.Project.Has(permission);
    }

    public bool HasServerPermission(TeamMemberData member, ServerPermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Server.ServerAdministrator)
            return true;

        return _Permissions.Server.Has(permission);
    }

    public bool HasWebsitePermission(TeamMemberData member, WebsitePermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Website.WebsiteAdministrator)
            return true;

        return _Permissions.Website.Has(permission);
    }

    public void Update(UpdateDefinition<TeamRoleData> update)
    {
        var filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        _DB.Roles.Collection.UpdateOne(filter, update);
    }
}
