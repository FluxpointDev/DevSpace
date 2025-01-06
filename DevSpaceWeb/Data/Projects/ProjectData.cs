using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;
using System;

namespace DevSpaceWeb.Data.Projects;

public class ProjectData : ITeamResource
{
    
    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }

    public async Task UpdateAsync(UpdateDefinition<ProjectData> update, Action action)
    {
        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Projects.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
