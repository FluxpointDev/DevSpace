namespace DevSpaceWeb.Fido2;

public class Fido2Request
{
    public Fido2Request(Guid userId, bool logRequest)
    {
        UserId = userId;
        LogRequest = logRequest;
    }
    public Guid UserId { get; set; }
    public bool LogRequest { get; set; }
    public bool IsSuccess { get; set; }
}
