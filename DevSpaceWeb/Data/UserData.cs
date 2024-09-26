
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data;

public class UserData
{
    [BsonId]
    public ObjectId Id { get; set; }


}
