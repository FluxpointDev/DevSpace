using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Links;

public class LinkData : ITeamResource
{
    public LinkData() : base(ResourceType.Link)
    {

    }

    public string ShortCode { get; set; }
    public string Link { get; set; }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<LinkData> filter = Builders<LinkData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Links.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.LinkDeleted)
                .SetTarget(this));

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}

public class LinkDataOld
{
    public static LinkDataOld Static = new LinkDataOld();

    public string Title = "Hello World!";
    public string Description = "Test description here";
    public string? BackgroundUrl;
    public bool RequireAuthentication;
    public bool RequireMember;
    public bool RequireRoles;
    public HashSet<ObjectId> RestrictedRoles = [];
    public int ButtonTimer = 3;
    public bool RequireCode;
    public string? Code;
    public string? Link;
    public bool IsDownload;

    public string GetDomain()
    {
        return Utils.GetDomain(Link)!;
    }
}