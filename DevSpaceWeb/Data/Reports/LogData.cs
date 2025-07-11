using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Reports;

public class LogData : IObject
{
    public LogData()
    {

    }

    public ObjectId TeamId { get; set; }

    public bool IsRead { get; set; }

    public bool IsSolved { get; set; }

    public ObjectId AssignedUser { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
