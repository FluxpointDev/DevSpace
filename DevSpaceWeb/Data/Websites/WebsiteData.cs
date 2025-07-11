using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Websites;

public class WebsiteData : ITeamResource
{
    public WebsiteData() : base(ResourceType.Website) { }

    public string Domain { get; set; }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Websites.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.WebsiteDeleted)
                .SetTarget(this));

            _DB.Websites.Cache.TryRemove(Id, out _);

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}
