namespace DevSpaceShared.WebSocket;

public class IWebSocketResponse<T> : IWebSocketEvent
{
    public IWebSocketResponse() : base(EventType.TaskResponse)
    {

    }

    public bool IsSuccess { get; set; }

    public string TaskId { get; set; }

    public T? Data { get; set; }
}

public class IWebSocketTask : IWebSocketEvent
{
    public IWebSocketTask(EventType type) : base(type)
    {

    }

    public string TaskId;
}

public class IWebSocketEvent
{
    public IWebSocketEvent(EventType type)
    {
        this.Type = type;
    }

    public EventType Type;
}
