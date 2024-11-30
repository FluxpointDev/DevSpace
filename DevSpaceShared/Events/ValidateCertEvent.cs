namespace DevSpaceShared.WebSocket;
public class ValidateCertEvent : IWebSocketEvent
{
    public ValidateCertEvent() : base(EventType.ValidateCert)
    {

    }

    public string CertHash;
    public bool IsRunningDockerContainer;
}
