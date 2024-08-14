using MongoDB.Bson;

namespace DevSpaceWeb.Fido2;

public class AuthRequest
{
    public AuthRequest(ObjectId userId, bool logRequest)
    {
        UserId = userId;
        LogRequest = logRequest;
    }
    public ObjectId UserId { get; set; }
    public bool LogRequest { get; set; }
    public bool IsSuccess { get; set; }
}
