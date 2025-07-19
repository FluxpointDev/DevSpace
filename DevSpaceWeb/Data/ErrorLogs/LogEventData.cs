using DevSpaceWeb.Data.Teams;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.ErrorLogs;

public class LogEventData : IBaseObject
{
    public static LogEventData Create(SentryEvent data)
    {
        LogEventData Event = new LogEventData
        {

        };

        return Event;
    }

    public ObjectId LogId { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public string Json { get; set; }
}
