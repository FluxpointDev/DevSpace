using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Projects;

public class ProjectData
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

    public void Update(UpdateDefinition<ProjectData> update)
    {
        var filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
        _DB.Projects.Collection.UpdateOne(filter, update);
    }
}
