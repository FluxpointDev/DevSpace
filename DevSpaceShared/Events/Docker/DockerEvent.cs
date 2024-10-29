using DevSpaceShared.WebSocket;

namespace DevSpaceShared.Events.Docker;

public class DockerEvent : IWebSocketTaskEvent
{
    public DockerEvent(DockerEventType type, string? resourceId = null, ControlContainerType? controlType = null) : base(EventType.Docker)
    {
        DockerType = type;
        ResourceId = resourceId;
        ControlType = controlType;
    }

    public string? ResourceId { get; set; }
    public string[]? ResourceList { get; set; }

    public DockerEventType DockerType { get; set; }

    public ControlContainerType? ControlType { get; set; }

    public object? Data { get; set; }
}
public enum DockerEventType
{
    ListContainers, GetContainer, InspectContainer, CreateContainer, UpdateContainer, ControlContainer,
    ListImages, DeleteImage,
    SystemInfo
}
public enum ControlContainerType
{
    Kill, Start, Stop, Pause, UnPause, Restart, Remove
}