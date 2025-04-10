﻿using DevSpaceAgent.Docker;
using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using DevSpaceShared.Responses;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text;

namespace DevSpaceAgent.Server;
public static class DockerHandler
{
    public static async Task<object?> Run(DockerEvent @event)
    {
        Console.WriteLine($"Run Docker Event {@event.DockerType.ToString()}");
        DockerClient Client = Program.DockerClient!;


        switch (@event.DockerType)
        {
            // Stacks

            case DockerEventType.ListStacks:
                return await DockerStacks.ListStacksAsync(Client);
            case DockerEventType.CreateStack:
                return await DockerStacks.CreateStackAsync(Client, @event.Data.ToObject<CreateStackEvent>());
            case DockerEventType.ControlStack:
                {
                    switch (@event.StackType)
                    {
                        case ControlStackType.View:
                            return await DockerStacks.ViewStack(Client, @event.ResourceId);
                        case ControlStackType.Start:
                        case ControlStackType.Stop:
                        case ControlStackType.Pause:
                        case ControlStackType.Resume:
                        case ControlStackType.Restart:
                            if (@event.ResourceList == null)
                                await DockerStacks.ControlStack(Client, @event.ResourceId, @event.StackType.Value);
                            else
                            {
                                foreach (string i in @event.ResourceList)
                                {
                                    await DockerStacks.ControlStack(Client, i, @event.StackType.Value);
                                }
                            }
                            break;

                        case ControlStackType.Remove:
                            if (@event.ResourceList == null)
                                await DockerStacks.RemoveStack(Client, @event.ResourceId);
                            else
                            {
                                foreach (string i in @event.ResourceList)
                                {
                                    await DockerStacks.RemoveStack(Client, i);
                                }
                            }
                            break;
                        case ControlStackType.ComposeInfo:
                            return await DockerStacks.StackCompose(Client, @event.ResourceId);
                        case ControlStackType.ReCreate:
                            return await DockerStacks.RecreateContainer(Client, @event.ResourceId, @event.Data.ToObject<CreateStackEvent>());
                        case ControlStackType.ListPortainer:
                            return await DockerStacks.ListPortainerStacks(Client);
                        case ControlStackType.ImportPortainer:
                            return await DockerStacks.ImportPortainerStack(Client, @event.ResourceId);
                    }
                }
                break;
            // Containers

            case DockerEventType.ListContainers:
                return await DockerContainers.ListContainersAsync(Client);

            case DockerEventType.CreateContainer:
                return await DockerContainers.CreateContainerAsync(Client, @event);



            case DockerEventType.ControlContainer:
                if (@event.ResourceList == null)
                    return await DockerContainers.ControlContainerAsync(Client, @event, @event.ResourceId);
                else
                {
                    foreach (string i in @event.ResourceList)
                    {
                        await DockerContainers.ControlContainerAsync(Client, @event, i);
                    }
                }
                break;

            // Images

            case DockerEventType.ListImages:
                return await DockerImages.ListImagesAsync(Client);

            case DockerEventType.ControlImage:
                if (@event.ResourceList == null)
                    return await DockerImages.ControlImageAsync(Client, @event, @event.ResourceId);
                else
                {
                    foreach (string i in @event.ResourceList)
                    {
                        await DockerImages.ControlImageAsync(Client, @event, i);
                    }
                }
                break;


            case DockerEventType.SearchImages:
                return await DockerImages.SearchImagesAsync(Client, @event);

            case DockerEventType.PruneImages:
                return await DockerImages.PruneImagesAsync(Client);

            case DockerEventType.GetPullLimit:
                return new RegistryLimitResponse
                {
                    MaxLimit = Program.State.MaxPullLimit,
                    CurrentLimit = Program.State.CurrentPullLimit
                };

            case DockerEventType.PullImage:
                await DockerImages.PullImageAsync(Client, @event.ResourceId);
                break;
            case DockerEventType.CreateImage:
                await DockerImages.CreateImageAsync(Client, @event.Data.ToObject<CreateImageEvent>());
                break;

            // Plugins

            case DockerEventType.ListPlugins:
                return await DockerPlugins.ListPluginsAsync(Client);

            case DockerEventType.ControlPlugin:
                if (@event.ResourceList == null)
                    return await DockerPlugins.ControlPluginAsync(Client, @event, @event.ResourceId);
                else
                {
                    foreach (string i in @event.ResourceList)
                    {
                        await DockerPlugins.ControlPluginAsync(Client, @event, i);
                    }
                }
                break;

            // Networks

            case DockerEventType.ListNetworks:
                return await DockerNetworks.ListNetworksAsync(Client);

            case DockerEventType.ControlNetwork:
                if (@event.ResourceList == null)
                    return await DockerNetworks.ControlNetworkAsync(Client, @event, @event.ResourceId);
                else
                {
                    foreach (string i in @event.ResourceList)
                    {
                        await DockerNetworks.ControlNetworkAsync(Client, @event, i);
                    }
                }
                break;

            // Volumes


            case DockerEventType.ListVolumes:
                return await DockerVolumes.ListVolumesAsync(Client);

            case DockerEventType.CreateVolume:
                return await DockerVolumes.CreateVolumeAsync(Client, @event.Data.ToObject<VolumesCreateParameters>());

            case DockerEventType.ControlVolume:
                if (@event.ResourceList == null)
                    return await DockerVolumes.ControlVolumeAsync(Client, @event, @event.ResourceId);
                else
                {
                    foreach (string i in @event.ResourceList)
                    {
                        await DockerVolumes.ControlVolumeAsync(Client, @event, i);
                    }
                }
                break;

            // Custom Templates
            case DockerEventType.ListCustomTemplates:
                return DockerCustomTemplates.ListTemplates();
            case DockerEventType.CreateCustomTemplate:
                DockerCustomTemplates.CreateTemplate(@event.Data.ToObject<CreateCustomTemplateEvent>());
                break;
            case DockerEventType.ControlCustomTemplate:
                return DockerCustomTemplates.ControlTemplate(@event);
            case DockerEventType.ImportPortainerTemplates:
                DockerCustomTemplates.ImportPortainerTemplates();
                break;

            // Other

            case DockerEventType.SystemInfo:
                return await DockerSystem.GetSystemInfoAsync(Client);
            case DockerEventType.HostInfo:
                return await DockerSystem.GetHostInfoAsync(Client);
            case DockerEventType.Events:
                DockerEventFilterEvent? Filter = null;
                if (@event.Data != null)
                    Filter = @event.Data.ToObject<DockerEventFilterEvent>();

                ContainerEventsParameters Params = new ContainerEventsParameters
                {
                    Until = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString()
                };

                if (Filter != null)
                {
                    Params.Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        { Filter.FilterType, new Dictionary<string, bool> { { Filter.FilterId, true } } }
                    };
                }

                Stream Stream = await Program.DockerClient.System.MonitorEventsAsync(Params, CancellationToken.None);
                List<DockerEventInfo> List = new List<DockerEventInfo>();
                using (StreamReader reader = new StreamReader(Stream, Encoding.UTF8, true))
                {
                    while (!reader.EndOfStream)
                    {
                        string? Line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(Line))
                        {
                            DockerEventInfo? Info = Newtonsoft.Json.JsonConvert.DeserializeObject<DockerEventInfo>(Line);
                            if (Info != null)
                                List.Add(Info);
                        }
                    }
                }
                return List;
        }
        return null;
    }
}
