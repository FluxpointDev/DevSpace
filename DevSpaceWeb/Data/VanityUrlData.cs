using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Data;

public class VanityUrlData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ConcurrentDictionary<string, ObjectId> ServerVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> ProjectVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> WebsiteVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> LogsVanityUrls = new ConcurrentDictionary<string, ObjectId>();
}
