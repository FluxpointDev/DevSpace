using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Projects;

public class ProjectData : ITeamResource
{
    public ProjectData() : base(ResourceType.Project) { }

    public ulong CurrentIssueNumber { get; set; }

    public string EncryptedLogKey { get; set; }

    [BsonIgnore]
    private string? DecryptedLogKey;

    public void ResetDecryptedLogKey()
    {
        DecryptedLogKey = null;
    }
    public string GetDecryptedLogKey()
    {
        if (string.IsNullOrEmpty(DecryptedLogKey))
            DecryptedLogKey = Crypt.DecryptString(EncryptedLogKey);

        return DecryptedLogKey;
    }

    public async Task UpdateAsync(UpdateDefinition<ProjectData> update, Action? action = null)
    {
        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Projects.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Projects.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.ProjectDeleted)
                .SetTarget(this));

            _DB.Projects.Cache.TryRemove(Id, out _);

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }

    [BsonIgnore]
    [JsonIgnore]
    public SemaphoreSlim LogLock = new SemaphoreSlim(1, 1);
}
public enum ProjectPlatformType
{
    As, C, Cfml, Cocoa, CSharp, Elixir, Haskell, Go, Groovy, Java, JavaScript, Native, Node, Objc, Other, Perl, Php, Python, Ruby
}