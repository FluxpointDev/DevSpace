using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Projects;

public class ProjectData : ITeamResource
{
    public async Task UpdateAsync(UpdateDefinition<ProjectData> update, Action action)
    {
        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Projects.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
