using DevSpaceShared.WebSocket;

namespace DevSpaceShared.Events.Docker;

public class DockerEvent : IWebSocketTaskEvent
{
    public DockerEvent(DockerEventType type, string? resourceId = null, ControlContainerType? containerType = null,
        ControlPluginType? pluginType = null, ControlImageType? imageType = null, ControlStackType? stackType = null,
        ControlNetworkType? networkType = null, ControlVolumeType? volumeType = null) : base(EventType.Docker)
    {
        DockerType = type;
        ResourceId = resourceId;
        ContainerType = containerType;
        PluginType = pluginType;
        ImageType = imageType;
        StackType = stackType;
        NetworkType = networkType;
        VolumeType = volumeType;
    }

    public string? ResourceId { get; set; }
    public string[]? ResourceList { get; set; }

    public DockerEventType DockerType { get; set; }

    public ControlContainerType? ContainerType { get; set; }

    public ControlPluginType? PluginType { get; set; }

    public ControlImageType? ImageType { get; set; }

    public ControlStackType? StackType { get; set; }

    public ControlNetworkType? NetworkType { get; set; }

    public ControlVolumeType? VolumeType { get; set; }

    public object? Data { get; set; }
}
public enum DockerEventType
{
    ListContainers, GetContainer, InspectContainer, CreateContainer, UpdateContainer, ControlContainer,
    ListImages, ControlImage, SearchImages, PruneImages,
    ListPlugins, ControlPlugin,
    ListStacks, ControlStack,
    ListNetworks, ControlNetwork,
    ListVolumes, ControlVolume,
    SystemInfo
}
public enum ControlContainerType
{
    Kill, Start, Stop, Pause, UnPause, Restart, Remove
}
public enum ControlPluginType
{
    Enable, Disable, Remove, InstallCheck, InstallFull
}
public enum ControlImageType
{
    Export, Remove, RemoveForce, Inspect
}
public enum ControlStackType
{
    Remove
}
public enum ControlNetworkType
{
    Remove
}
public enum ControlVolumeType
{
    Remove
}