using DevSpaceShared.WebSocket;

namespace DevSpaceShared.Events.Docker;
public class ControlContainerEvent : ContainerEvent
{
    public ControlContainerEvent(string containerId, ControlContainerType type) : base(EventType.DockerControlContainer, containerId)
    {
        ControlType = type;
    }

    public ControlContainerType ControlType { get; set; }
}
public enum ControlContainerType
{
    Kill, Start, Stop, Pause, UnPause, Restart, Remove
}