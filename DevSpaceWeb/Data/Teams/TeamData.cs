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
    public Guid? ResourceId { get; set; }
    public Guid? IconId { get; set; }

    [BsonIgnore]
    public FileResource Icon => new FileResource("Icon", ResourceId, IconId);

    [BsonIgnore]
    public bool HasIcon => IconId != null;

    public string GetIconOrDefault(bool usePng = false)
    {
        if (!HasIcon)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return Icon.Url(usePng ? "png" : "webp");
    }

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();

    public PermissionsAll GetPermissions() { return new PermissionsAll(DefaultPermissions); }

    public HashSet<ObjectId> Roles = new HashSet<ObjectId>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, ObjectId> Members = new Dictionary<ObjectId, ObjectId>();

    public TeamMemberData GetMember(PartialUserData user)
    {
        return CachedMembers[Members[user.Id]];
    }

    public TeamMemberData GetMember(AuthUser user)
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
        FilterDefinition<TeamData> filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
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

    public bool HasDockerPermission(TeamMemberData member, DockerPermission permission)
    {
        if (member.Team == null)
            return false;

        if (_Permissions.Team.GlobalAdministrator || _Permissions.Docker.DockerAdministrator)
            return true;

        return _Permissions.Docker.Has(permission);
    }

    public void Update(UpdateDefinition<TeamRoleData> update)
    {
        FilterDefinition<TeamRoleData> filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        _DB.Roles.Collection.UpdateOne(filter, update);
    }
}
