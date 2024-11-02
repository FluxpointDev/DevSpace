using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet.Models;
using System;

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
                    switch (@event.ContainerType)
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
                    All = true
                });
            case DockerEventType.ControlImage:
                {
                    switch (@event.ImageType)
                    {
                        case ControlImageType.Export:
                            
                            break;
                        case ControlImageType.Remove:
                        case ControlImageType.RemoveForce:
                            {
                                if (@event.ResourceList != null)
                                {
                                    foreach (var r in @event.ResourceList)
                                    {
                                        await Program.DockerClient.Images.DeleteImageAsync(r, new ImageDeleteParameters
                                        {
                                            Force = @event.ImageType == ControlImageType.RemoveForce
                                        });
                                    }
                                }
                                else
                                {
                                    await Program.DockerClient.Images.DeleteImageAsync(@event.ResourceId, new ImageDeleteParameters
                                    {
                                        Force = @event.ImageType == ControlImageType.RemoveForce
                                    });
                                }
                            }
                            break;
                        case ControlImageType.Inspect:
                            return await Program.DockerClient.Images.InspectImageAsync(@event.ResourceId);
                    }
                }
                break;
            case DockerEventType.SearchImages:
                return await Program.DockerClient.Images.SearchImagesAsync(new ImagesSearchParameters
                {
                    Term = @event.ResourceId,
                    Limit = 15
                });
            case DockerEventType.PruneImages:
                return await Program.DockerClient.Images.PruneImagesAsync(new ImagesPruneParameters
                {
                    
                });
            case DockerEventType.ListPlugins:
                return await Program.DockerClient.Plugin.ListPluginsAsync(new PluginListParameters
                {
                    
                });
            case DockerEventType.ListNetworks:
                return await Program.DockerClient.Networks.ListNetworksAsync();
            case DockerEventType.ListStacks:
                {
                    List<DockerStack> Stacks = new List<DockerStack>();
                    var containers = await Program.DockerClient.Containers.ListContainersAsync(new ContainersListParameters()
                    {
                        Size = true,
                        All = true
                    });

                    foreach(var c in containers)
                    {
                        if (!c.Labels.TryGetValue("com.docker.compose.project", out string label))
                            continue;

                        var Stack = Stacks.FirstOrDefault(x => x.Name == label);
                        if (Stack != null)
                            Stack.Containers.Add(c.Names.First().Substring(1));
                    }

                    return Stacks;
                }
                break;
            case DockerEventType.ListVolumes:
                return await Program.DockerClient.Volumes.ListAsync();
            case DockerEventType.ControlPlugin:
                {
                    switch (@event.PluginType)
                    {
                        case ControlPluginType.InstallCheck:
                            return await Program.DockerClient.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                            {
                                Remote = @event.ResourceId
                            });
                        case ControlPluginType.InstallFull:
                            {
                                var Privs = await Program.DockerClient.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                                {
                                    Remote = @event.ResourceId
                                });
                                string ErrorMessage = "";
                                var progress = new Progress<JSONMessage>(msg =>
                                {
                                    ErrorMessage = msg.ErrorMessage;
                                });
                                await Program.DockerClient.Plugin.InstallPluginAsync(new PluginInstallParameters
                                {
                                    Remote = @event.ResourceId,
                                    Privileges = Privs
                                }, progress);

                                if (!string.IsNullOrEmpty(ErrorMessage))
                                    throw new Exception(ErrorMessage);
                            }
                            break;
                    }
                }
                break;
            case DockerEventType.SystemInfo:
                return await Program.DockerClient.System.GetSystemInfoAsync();
        }

        return null;
    }
}
