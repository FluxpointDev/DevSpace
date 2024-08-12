namespace DevSpaceShared.WebSocket;

public class IWebSocketResponseEvent<T> : IWebSocketEvent
{
    public IWebSocketResponseEvent() : base(EventType.TaskResponse)
    {

    }

    public T Data { get; set; }

    public bool IsFail { get; set; } = false;

    public string TaskId;
}

public class IWebSocketTaskEvent : IWebSocketEvent
{
    public IWebSocketTaskEvent(EventType type) : base(type)
    {

    }

    public string TaskId;
}
public class IWebSocketEvent
{
    public IWebSocketEvent(EventType type)
    {
        this.type = type;
    }

    public EventType type;
}
