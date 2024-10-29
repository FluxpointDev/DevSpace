using DevSpaceShared.Events.Docker;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Server;
public static class DockerHandler
{
    public static async Task<object?> Run(DockerEvent @event)
    {
        switch (@event.DockerType)
        {
            case DockerEventType.ListContainers:
                return await Program.DockerClient.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    Size = true,
                    All = true
                });
            case DockerEventType.GetContainer:
                {
                    var containers = await Program.DockerClient.Containers.ListContainersAsync(new ContainersListParameters()
                    {
                        Size = true,
                        All = true,
                        Filters = new Dictionary<string, IDictionary<string, bool>>
                                    {
                                        { "id", new Dictionary<string, bool>
                                        {
                                            { @event.ResourceId, true }
                                        }
                                        }
                                    }
                    });
                    if (containers.Any())
                        return containers.First();
                }
                break;
            case DockerEventType.InspectContainer:
                return await Program.DockerClient.Containers.InspectContainerAsync(@event.ResourceId);
            case DockerEventType.CreateContainer:
                return await Program.DockerClient.Containers.CreateContainerAsync(@event.Data as CreateContainerParameters);
            case DockerEventType.UpdateContainer:
                return await Program.DockerClient.Containers.UpdateContainerAsync(@event.ResourceId, @event.Data as ContainerUpdateParameters);
            case DockerEventType.ControlContainer:
                {
                    switch (@event.ControlType)
                    {
                        case ControlContainerType.Kill:
                            if (@event.ResourceList != null)
                            {
                                foreach(var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.KillContainerAsync(r, new ContainerKillParameters
                                    {
                                    });
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.KillContainerAsync(@event.ResourceId, new ContainerKillParameters
                                {
                                });
                            }

                            
                            break;
                        case ControlContainerType.Start:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.StartContainerAsync(r, new ContainerStartParameters
                                    {
                                    });
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.StartContainerAsync(@event.ResourceId, new ContainerStartParameters
                                {
                                });
                            }

                            
                            break;
                        case ControlContainerType.Stop:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.StopContainerAsync(r, new ContainerStopParameters
                                    {
                                    });
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.StopContainerAsync(@event.ResourceId, new ContainerStopParameters
                                {
                                });
                            }

                            
                            break;
                        case ControlContainerType.Restart:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.RestartContainerAsync(r, new ContainerRestartParameters
                                    {

                                    });
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.RestartContainerAsync(@event.ResourceId, new ContainerRestartParameters
                                {

                                });
                            }

                            
                            break;
                        case ControlContainerType.Remove:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.RemoveContainerAsync(r, new ContainerRemoveParameters
                                    {
                                        
                                    });
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.RemoveContainerAsync(@event.ResourceId, new ContainerRemoveParameters
                                {

                                });
                            }

                            
                            break;
                        case ControlContainerType.Pause:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.PauseContainerAsync(r);
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.PauseContainerAsync(@event.ResourceId);
                            }

                            
                            break;
                        case ControlContainerType.UnPause:
                            if (@event.ResourceList != null)
                            {
                                foreach (var r in @event.ResourceList)
                                {
                                    await Program.DockerClient.Containers.UnpauseContainerAsync(r);
                                }
                            }
                            else
                            {
                                await Program.DockerClient.Containers.UnpauseContainerAsync(@event.ResourceId);
                            }

                            
                            break;
                    }
                }
                break;
            case DockerEventType.ListImages:
                return await Program.DockerClient.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = true,
                });
            case DockerEventType.DeleteImage:
                await Program.DockerClient.Images.DeleteImageAsync(@event.ResourceId, new ImageDeleteParameters
                {

                });
                break;
            case DockerEventType.ListPlugins:
                return await Program.DockerClient.Plugin.ListPluginsAsync(new PluginListParameters
                {

                });
            case DockerEventType.SystemInfo:
                return await Program.DockerClient.System.GetSystemInfoAsync();
        }

        return null;
    }
}
