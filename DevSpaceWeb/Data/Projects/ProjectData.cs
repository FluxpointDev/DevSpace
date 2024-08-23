using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    //public async Task UpdateAsync()
    //{
    //    var update = Builders<ProjectData>.Update.Set(r => r.TeamId, TeamId);
    //    var filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
    //    await _DB.Projects.Collection.UpdateOne(filter, update);
    //}
}
