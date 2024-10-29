namespace DevSpaceShared.WebSocket;

public enum EventType
{
    Invalid,
    // Sent from the server to client and back to server to validate the certificate hash
    ValidateCert,
    Ping,
    Pong,
    TaskResponse,
    Command,
    CommandWait,
    CommandStream,
    Docker,
    FirewallInfo,
    SystemInfo
}
