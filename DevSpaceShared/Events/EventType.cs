namespace DevSpaceShared.WebSocket;

public enum EventType
{
    Invalid,
    GetAgentStats,
    Ping,
    Pong,
    TaskResponse,
    Docker,
    GetAgentOptions,
    UpdateAgentOptions
}
