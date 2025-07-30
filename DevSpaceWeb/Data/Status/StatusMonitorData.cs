using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Status;

public class StatusMonitorData : ITeamResource
{
    public StatusMonitorData() : base(ResourceType.StatusMonitor)
    {

    }

    public StatusMonitorType MonitorType { get; set; }

    public string Source { get; set; }

    public async Task UpdateAsync(UpdateDefinition<StatusMonitorData> update, Action action)
    {
        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.StatusMonitors.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.StatusMonitors.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.StatusMonitorDeleted)
                .SetTarget(this));

            _DB.StatusMonitors.Cache.TryRemove(Id, out _);

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}
public enum StatusMonitorType
{
    Ping, Http
}
