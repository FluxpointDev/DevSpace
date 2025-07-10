using DevSpaceWeb.Data.Teams;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.Reports;

public class LogData : IObject
{
    public LogData()
    {

    }

    public ObjectId TeamId { get; set; }
}
