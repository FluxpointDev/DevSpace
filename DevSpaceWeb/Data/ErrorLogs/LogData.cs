using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Reports;

public class LogData : IObject
{
    public LogData()
    {

    }

    public ObjectId TeamId { get; set; }

    public ObjectId ProjectId { get; set; }

    public ulong IssueNumber { get; set; }

    public ulong EventsCount { get; set; }

    public bool IsRead { get; set; }

    public ObjectId? AssignedUser { get; set; }

    public string MessageTitle { get; set; }

    public string Message { get; set; }

    public LogType LogType { get; set; }

    public LogStatus Status { get; set; }

    public List<LogActivity> Activity = new List<LogActivity>();

    public string Hash { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public DateTime CreatedAt { get; set; }

    public DateTime LastSeenAt { get; set; }

    public async Task UpdateAsync(UpdateDefinition<LogData> update, Action? action = null)
    {
        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Logs.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
public enum LogType
{
    Trace, Debug, Info, Warn, Error, Fatal
}
public enum LogStatus
{
    New, Open, Reviewed, Ignored, Resolved
}

public class LogActivity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public LogActivityType ActivityType { get; set; }
    public ObjectId ActionUserId { get; set; }
    public ObjectId? TargetUserId { get; set; }
    public string? OtherUsername { get; set; }

    [BsonIgnore]
    [JsonIgnore]
    public string GetText => ActivityType switch
    {
        LogActivityType.AssignedUser => (_DB.Users.TryGetValue(ActionUserId, out PartialUserData? user) ? user.GetCurrentName() : "Unknown User?") + " assigned issue to " + (TargetUserId.HasValue && _DB.Users.TryGetValue(TargetUserId.Value, out PartialUserData? target) ? target.GetCurrentName() : "Unknown User?"),
        LogActivityType.CommentAdded => (_DB.Users.TryGetValue(ActionUserId, out PartialUserData? user) ? user.GetCurrentName() : "Unknown User?") + " added comment",
        LogActivityType.Ignored => (_DB.Users.TryGetValue(ActionUserId, out PartialUserData? user) ? user.GetCurrentName() : "Unknown User?") + " ignored this issue",
        LogActivityType.ReportAdded => "Report added by " + OtherUsername,
        LogActivityType.Resolved => (_DB.Users.TryGetValue(ActionUserId, out PartialUserData? user) ? user.GetCurrentName() : "Unknown User?") + " resolved this issue",
        LogActivityType.Reviewed => (_DB.Users.TryGetValue(ActionUserId, out PartialUserData? user) ? user.GetCurrentName() : "Unknown User?") + " reviewed this issue",
        LogActivityType.Opened => "Automatically opened by new event"
    };

    [BsonIgnore]
    [JsonIgnore]
    public string GetIcon => ActivityType switch
    {
        LogActivityType.AssignedUser => "person_add",
        LogActivityType.CommentAdded => "add_comment",
        LogActivityType.Ignored => "alarm_pause",
        LogActivityType.ReportAdded => "add_notes",
        LogActivityType.Resolved => "check",
        LogActivityType.Reviewed => "person_alert",
        LogActivityType.Opened => "exclamation"
    };
}
public enum LogActivityType
{
    Resolved, Ignored, AssignedUser, CommentAdded, ReportAdded, Reviewed, Opened
}