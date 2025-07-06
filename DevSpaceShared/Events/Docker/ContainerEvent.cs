using DevSpaceShared.WebSocket;
using System.Text.Json.Serialization;
namespace DevSpaceShared.Events.Docker;

public class ContainerEvent : IWebSocketTask
{
    public ContainerEvent(EventType type, string containerId) : base(type)
    {
        ContainerId = containerId;
    }

    [JsonConstructor]
    internal ContainerEvent() : base()
    {

    }

    public string ContainerId { get; set; }
}
