using DevSpaceShared.WebSocket;
namespace DevSpaceShared.Events.Docker;

public class ContainerEvent : IWebSocketTaskEvent
{
    public ContainerEvent(EventType type, string containerId) : base(type)
    {
        ContainerId = containerId;
    }

    public string ContainerId { get; set; }
}
