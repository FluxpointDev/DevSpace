using DevSpaceShared.WebSocket;
using Docker.DotNet.Models;

namespace DevSpaceShared.Events.Docker;
public class UpdateContainerEvent : ContainerEvent
{
    public UpdateContainerEvent(string containerId, ContainerUpdateParameters param) : base(EventType.DockerCreateContainer, containerId)
    {
        Params = param;
    }

    public ContainerUpdateParameters Params { get; set; }
}
