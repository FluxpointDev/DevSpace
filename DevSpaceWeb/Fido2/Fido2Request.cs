using MongoDB.Bson;

namespace DevSpaceWeb.Fido2;

public class Fido2Request
{
    public Fido2Request(ObjectId userId, bool logRequest)
    {
        UserId = userId;
        LogRequest = logRequest;
    }
    public ObjectId UserId { get; set; }
    public bool LogRequest { get; set; }
    public bool IsSuccess { get; set; }
}
