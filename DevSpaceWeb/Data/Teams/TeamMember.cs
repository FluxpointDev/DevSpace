using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
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
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId.ToString());

    public HashSet<ObjectId> Roles { get; set; } = new HashSet<ObjectId>();
    public string NickName { get; set; }
    public Guid? AvatarId { get; set; }

    public void Update(UpdateDefinition<TeamMemberData> update)
    {
        var filter = Builders<TeamMemberData>.Filter.Eq(r => r.Id, Id);
        _DB.Members.Collection.UpdateOne(filter, update);
    }
}
