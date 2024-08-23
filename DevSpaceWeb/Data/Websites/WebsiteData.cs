using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data.Websites;

public class WebsiteData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }

    public string Domain { get; set; }
    public ObjectId TeamId { get; set; }
    public bool HasAccess(SessionProvider session)
    {
        return true;
    }

    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }
}
