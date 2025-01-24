using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class IResource
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ObjectId OwnerId { get; set; }
}
public class ITeamResource : IResource
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> MemberPermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> RolePermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> ApiPermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();

    public string? VanityUrl { get; set; }

    public string GetVanityUrlOrId()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public ObjectId TeamId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);
}