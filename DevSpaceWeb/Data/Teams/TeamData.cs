using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Teams;

public class TeamData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string VanityUrl { get; set; }
    public ObjectId OwnerId { get; set; }

    public Dictionary<ObjectId, TeamUserData> Users = new Dictionary<ObjectId, TeamUserData>();

    public bool HasAccess(SessionProvider session)
    {
        return true;
    }

    public string GetVanityUrl()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public void Update()
    {
        var update = Builders<TeamData>.Update.Set(r => r.Id, Id);
        var filter = Builders<TeamData>.Filter.Eq(r => r.Id, Id);
        _DB.Teams.Collection.UpdateOne(filter, update);
    }
}
public class TeamUserData
{
    public ObjectId Id { get; set; }
    public TeamPermissions Permissions { get; set; } = new TeamPermissions();
    public string NickName { get; set; }

    public Guid? AvatarId { get; set; }
}
