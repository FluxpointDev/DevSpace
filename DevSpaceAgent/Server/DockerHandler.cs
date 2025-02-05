using DevSpaceAgent.Docker;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;

namespace DevSpaceAgent.Server;
public static class DockerHandler
{
    public static async Task<object?> Run(DockerEvent @event)
    {
        DockerClient Client = Program.DockerClient;
        switch (@event.DockerType)
        {
            case DockerEventType.ListContainers:
                return await DockerContainers.ListContainersAsync(Client);

            case DockerEventType.GetContainer:
                return await DockerContainers.GetContainerAsync(Client, @event.ResourceId);

            case DockerEventType.InspectContainer:
                return await DockerContainers.InspectContainerAsync(Client, @event.ResourceId);

            case DockerEventType.CreateContainer:
                return await DockerContainers.CreateContainerAsync(Client, @event);

            case DockerEventType.UpdateContainer:
                return await DockerContainers.UpdateContainerAsync(Client, @event);

            case DockerEventType.ControlContainer:
                await DockerContainers.ControlContainerAsync(Client, @event);
                break;

            case DockerEventType.ListImages:
                return await DockerImages.ListImagesAsync(Client);

            case DockerEventType.ControlImage:
                return await DockerImages.ControlImageAsync(Client, @event);

            case DockerEventType.SearchImages:
                return await DockerImages.SearchImagesAsync(Client, @event);

            case DockerEventType.PruneImages:
                return await DockerImages.PruneImagesAsync(Client);

            case DockerEventType.ListPlugins:
                return await DockerPlugins.ListPluginsAsync(Client);

            case DockerEventType.ListNetworks:
                return await DockerNetworks.ListNetworksAsync(Client);

            case DockerEventType.ListStacks:
                return await DockerStacks.ListStacksAsync(Client);

            case DockerEventType.ListVolumes:
                return await DockerVolumes.ListVolumesAsync(Client);

            case DockerEventType.ControlPlugin:
                return await DockerPlugins.ControlPluginAsync(Client, @event);

            case DockerEventType.SystemInfo:
                return await DockerSystem.GetSystemInfoAsync(Client);
        }

        return null;
    }
}
