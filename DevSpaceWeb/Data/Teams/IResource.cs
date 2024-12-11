using DevSpaceWeb.Data.Permissions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.Teams;

public class IResource
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId OwnerId { get; set; }
}
public class ITeamResource : IResource
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> MemberPermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> RolePermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    public ObjectId TeamId { get; set; }
}