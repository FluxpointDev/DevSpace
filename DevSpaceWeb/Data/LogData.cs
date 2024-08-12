using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data;

public class LogData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public ObjectId TeamId { get; set; }

    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }
}
