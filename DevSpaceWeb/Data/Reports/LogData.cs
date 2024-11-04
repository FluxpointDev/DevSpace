using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Reports;

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

    public void Update(UpdateDefinition<LogData> update)
    {
        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
        _DB.Logs.Collection.UpdateOne(filter, update);
    }
}
