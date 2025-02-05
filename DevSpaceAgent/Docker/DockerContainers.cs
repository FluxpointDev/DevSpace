using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerContainers
    {
        public static async Task<IList<ContainerListResponse>> ListContainersAsync(DockerClient client)
        {
            return await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                Size = true,
                All = true
            });
        }

        public static async Task<ContainerListResponse?> GetContainerAsync(DockerClient client, string id)
        {
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                Size = true,
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                                    {
                                        { "id", new Dictionary<string, bool>
                                        {
                                            { id, true }
                                        }
                                        }
                                    }
            });
            if (containers.Any())
                return containers.First();

            return null;
        }

        public static async Task<ContainerInspectResponse> InspectContainerAsync(DockerClient client, string id)
        {
            return await client.Containers.InspectContainerAsync(id);
        }

        public static async Task<CreateContainerResponse> CreateContainerAsync(DockerClient client, DockerEvent @event)
        {
            return await client.Containers.CreateContainerAsync(@event.Data as CreateContainerParameters);
        }

        public static async Task<ContainerUpdateResponse> UpdateContainerAsync(DockerClient client, DockerEvent @event)
        {
            return await client.Containers.UpdateContainerAsync(@event.ResourceId, @event.Data as ContainerUpdateParameters);
        }

        public static async Task ControlContainerAsync(DockerClient client, DockerEvent @event)
        {
            switch (@event.ContainerType)
            {
                case ControlContainerType.Kill:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.KillContainerAsync(r, new ContainerKillParameters
                            {
                            });
                        }
                    }
                    else
                    {
                        await client.Containers.KillContainerAsync(@event.ResourceId, new ContainerKillParameters
                        {
                        });
                    }


                    break;
                case ControlContainerType.Start:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.StartContainerAsync(r, new ContainerStartParameters
                            {
                            });
                        }
                    }
                    else
                    {
                        await client.Containers.StartContainerAsync(@event.ResourceId, new ContainerStartParameters
                        {
                        });
                    }


                    break;
                case ControlContainerType.Stop:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.StopContainerAsync(r, new ContainerStopParameters
                            {
                            });
                        }
                    }
                    else
                    {
                        await client.Containers.StopContainerAsync(@event.ResourceId, new ContainerStopParameters
                        {
                        });
                    }


                    break;
                case ControlContainerType.Restart:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.RestartContainerAsync(r, new ContainerRestartParameters
                            {

                            });
                        }
                    }
                    else
                    {
                        await client.Containers.RestartContainerAsync(@event.ResourceId, new ContainerRestartParameters
                        {

                        });
                    }


                    break;
                case ControlContainerType.Remove:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.RemoveContainerAsync(r, new ContainerRemoveParameters
                            {

                            });
                        }
                    }
                    else
                    {
                        await client.Containers.RemoveContainerAsync(@event.ResourceId, new ContainerRemoveParameters
                        {

                        });
                    }


                    break;
                case ControlContainerType.Pause:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.PauseContainerAsync(r);
                        }
                    }
                    else
                    {
                        await client.Containers.PauseContainerAsync(@event.ResourceId);
                    }


                    break;
                case ControlContainerType.UnPause:
                    if (@event.ResourceList != null)
                    {
                        foreach (string r in @event.ResourceList)
                        {
                            await client.Containers.UnpauseContainerAsync(r);
                        }
                    }
                    else
                    {
                        await client.Containers.UnpauseContainerAsync(@event.ResourceId);
                    }


                    break;
            }
        }
    }
}
