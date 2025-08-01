﻿using DevSpaceShared.WebSocket;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DevSpaceShared.Events.Docker;

public class DockerEvent : IWebSocketTask
{
    public DockerEvent(DockerEventType type, string? resourceId = null, ControlContainerType? containerType = null,
        ControlPluginType? pluginType = null, ControlImageType? imageType = null, ControlStackType? stackType = null,
        ControlNetworkType? networkType = null, ControlVolumeType? volumeType = null, ControlCustomTemplateType? customTemplateType = null) : base(EventType.Docker)
    {
        DockerType = type;
        ResourceId = resourceId;
        ContainerType = containerType;
        PluginType = pluginType;
        ImageType = imageType;
        StackType = stackType;
        NetworkType = networkType;
        VolumeType = volumeType;
        CustomTemplateType = customTemplateType;
    }

    [JsonConstructor]
    internal DockerEvent() : base()
    {

    }

    public string? ResourceId { get; set; }
    public string[]? ResourceList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DockerEventType DockerType { get; set; }

    public ControlContainerType? ContainerType { get; set; }

    public ControlPluginType? PluginType { get; set; }

    public ControlImageType? ImageType { get; set; }

    public ControlStackType? StackType { get; set; }

    public ControlNetworkType? NetworkType { get; set; }

    public ControlVolumeType? VolumeType { get; set; }

    public ControlCustomTemplateType? CustomTemplateType { get; set; }

    public JsonNode? Data { get; set; }
}
public enum DockerEventType
{
    ListContainers, CreateContainer, ControlContainer,
    ListImages, ControlImage, SearchImages, PruneImages, GetPullLimit, PullImage, CreateImage,
    ListPlugins, ControlPlugin,
    ListStacks, ControlStack, CreateStack,
    ListNetworks, ControlNetwork, CreateNetwork,
    ListVolumes, ControlVolume, CreateVolume,
    SystemInfo, HostInfo,
    ListCustomTemplates, CreateCustomTemplate, ControlCustomTemplate, ImportPortainerTemplates,
    Events, ListPortainerStacks
}
public enum ControlContainerType
{
    View,
    Inspect,
    Update,
    Kill,
    Start,
    Stop,
    Pause,
    UnPause,
    Restart,
    Remove,
    ForceRemove,
    Changes,
    Logs,
    Processes,
    Rename,
    Scan,
    ScanReport,
    Stats,
    Recreate
}
public enum ControlPluginType
{
    View,
    Inspect,
    Enable,
    Disable,
    Remove,
    ForceRemove,
    InstallCheck,
    InstallFull,
    Update
}
public enum ControlImageType
{
    View,
    Inspect,
    Layers,
    Export,
    Remove,
    ForceRemove
}
public enum ControlStackType
{
    View,
    Inspect,
    Start,
    Stop,
    Remove,
    Restart,
    Pause,
    Resume,
    ReCreate,
    ComposeInfo,
    ImportPortainer
}
public enum ControlNetworkType
{
    View,
    Inspect,
    Remove,
    LeaveNetwork,
    JoinNetwork
}
public enum ControlVolumeType
{
    View,
    Inspect,
    Remove,
    ForceRemove
}
public enum ControlCustomTemplateType
{
    ViewInfo,
    ViewFull,
    Inspect,
    ComposeInfo,
    EditInfo,
    EditCompose,
    Delete
}