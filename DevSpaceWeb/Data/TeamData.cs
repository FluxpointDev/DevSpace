using DevSpaceWeb.Components.Layout;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data;

public class TeamData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string VanityUrl { get; set; }
    public Guid OwnerId { get; set; }
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
}
