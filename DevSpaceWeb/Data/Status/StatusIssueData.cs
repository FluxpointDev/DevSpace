using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Status;

public class StatusIssueData : ITeamResource
{
    public StatusIssueData() : base(ResourceType.StatusIssue)
    {

    }

    public async Task UpdateAsync(UpdateDefinition<StatusIssueData> update, Action action)
    {
        FilterDefinition<StatusIssueData> filter = Builders<StatusIssueData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.StatusIssues.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<StatusIssueData> filter = Builders<StatusIssueData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.StatusIssues.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.StatusIssueDeleted)
                .SetTarget(this));

            _DB.StatusIssues.Cache.TryRemove(Id, out _);

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}
