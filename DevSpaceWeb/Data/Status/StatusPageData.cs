using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Status;

public class StatusPageData : ITeamResource
{
    public StatusPageData() : base(ResourceType.StatusPage)
    {

    }

    public async Task UpdateAsync(UpdateDefinition<StatusPageData> update, Action action)
    {
        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.StatusPages.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.StatusPages.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.StatusPageDeleted)
                .SetTarget(this));

            _DB.StatusPages.Cache.TryRemove(Id, out _);

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}
