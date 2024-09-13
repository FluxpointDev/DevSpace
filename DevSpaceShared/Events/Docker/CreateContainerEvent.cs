using DevSpaceShared.WebSocket;
using Docker.DotNet.Models;

namespace DevSpaceShared.Events.Docker;

public class CreateContainerEvent : IWebSocketTaskEvent
{
    public CreateContainerEvent(CreateContainerParameters param) : base(EventType.DockerCreateContainer)
    {
        Params = param;
    }

    public CreateContainerParameters Params { get; set; }
}
