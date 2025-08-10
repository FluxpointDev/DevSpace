using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Status;

public class StatusPageData : ITeamResource
{
    public StatusPageData() : base(ResourceType.StatusPage)
    {

    }

    public StatusPageCustomize Customize = new StatusPageCustomize();

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
public class StatusPageCustomize
{
    public string BackgroundColor { get; set; } = "#282727";

    public string HeaderColor { get; set; } = "#3A75CB";

    public string SectionColor { get; set; } = "#2F2F2F";

    public string OperationalColor { get; set; } = "#22DF20";

    public string DegradedColor { get; set; } = "#E3E120";

    public string PartialOutageColor { get; set; } = "#ED951D";

    public string MajorOutageColor { get; set; } = "#ED4628";

    public string MaintenanceColor { get; set; } = "#2FAED5";
}
