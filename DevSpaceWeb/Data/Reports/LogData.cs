using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Reports;

public class LogData : ITeamResource
{
    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }

    public async Task UpdateAsync(UpdateDefinition<LogData> update)
    {
        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Logs.Collection.UpdateOneAsync(filter, update);
        if (!Result.IsAcknowledged)
            throw new InvalidOperationException("Failed to update log data");
    }
}
