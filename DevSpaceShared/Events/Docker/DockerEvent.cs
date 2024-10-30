using DevSpaceShared.WebSocket;

namespace DevSpaceShared.Events.Docker;

public class DockerEvent : IWebSocketTaskEvent
{
    public DockerEvent(DockerEventType type, string? resourceId = null, ControlContainerType? containerType = null, ControlPluginType? pluginType = null) : base(EventType.Docker)
    {
        DockerType = type;
        ResourceId = resourceId;
        ContainerType = containerType;
        PluginType = pluginType;
    }

    public string? ResourceId { get; set; }
    public string[]? ResourceList { get; set; }

    public DockerEventType DockerType { get; set; }

    public ControlContainerType? ContainerType { get; set; }

    public ControlPluginType? PluginType { get; set; }

    public object? Data { get; set; }
}
public enum DockerEventType
{
    ListContainers, GetContainer, InspectContainer, CreateContainer, UpdateContainer, ControlContainer,
    ListImages, DeleteImage,
    ListPlugins, ControlPlugin,
    SystemInfo
}
public enum ControlContainerType
{
    Kill, Start, Stop, Pause, UnPause, Restart, Remove
}
public enum ControlPluginType
{
    Enable, Disable, Remove
}