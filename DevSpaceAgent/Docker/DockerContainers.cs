using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerContainers
    {
        public static async Task<IList<DockerContainerInfo>> ListContainersAsync(DockerClient client)
        {
            try
            {
                var Containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    Size = true,
                    All = true
                });
                return Containers.Select(x => DockerContainerInfo.Create(x)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
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

        public static async Task GetContainerLogsAsync(DockerClient client, string id)
        {
            MultiplexedStream Stream = await client.Containers.GetContainerLogsAsync(id, false, new ContainerLogsParameters
            {
                Tail = "100",
                ShowStdout = true,
                ShowStderr = true
            });
            await Stream.ReadOutputToEndAsync(CancellationToken.None);
        }


        public static async Task<CreateContainerResponse> CreateContainerAsync(DockerClient client, DockerEvent @event)
        {
            return await client.Containers.CreateContainerAsync(@event.Data.ToObject<CreateContainerParameters>());
        }

        public static async Task<ContainerUpdateResponse> UpdateContainerAsync(DockerClient client, DockerEvent @event)
        {
            return await client.Containers.UpdateContainerAsync(@event.ResourceId, @event.Data.ToObject<ContainerUpdateParameters>());
        }

        public static async Task<object?> ControlContainerAsync(DockerClient client, DockerEvent @event, string id)
        {
            switch (@event.ContainerType)
            {
                case ControlContainerType.View:
                    return await DockerContainers.GetContainerAsync(client, @event.ResourceId);
                case ControlContainerType.Update:
                    return await DockerContainers.UpdateContainerAsync(client, @event);
                case ControlContainerType.Inspect:
                    return await client.Containers.InspectContainerAsync(id);
                case ControlContainerType.Changes:
                    var Result = await client.Containers.InspectChangesAsync(id);
                    return new DockerContainerChanges
                    {
                        ContainerName = Program.ContainerCache.GetValueOrDefault(id, id),
                        Changes = Result
                    };
                case ControlContainerType.Kill:
                    await client.Containers.KillContainerAsync(id, new ContainerKillParameters
                    {
                    });
                    await client.Containers.WaitContainerAsync(id);
                    break;
                case ControlContainerType.Start:
                    await client.Containers.StartContainerAsync(id, new ContainerStartParameters
                    {

                    });
                    break;
                case ControlContainerType.Stop:
                    await client.Containers.StopContainerAsync(id, new ContainerStopParameters
                    {
                    });
                    await client.Containers.WaitContainerAsync(id);
                    break;
                case ControlContainerType.Restart:
                    await client.Containers.RestartContainerAsync(id, new ContainerRestartParameters
                    {

                    });
                    break;
                case ControlContainerType.ForceRemove:
                case ControlContainerType.Remove:
                    await client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters
                    {
                        Force = @event.ContainerType == ControlContainerType.ForceRemove
                    });
                    break;
                case ControlContainerType.Pause:
                    await client.Containers.PauseContainerAsync(id);
                    break;
                case ControlContainerType.UnPause:
                    await client.Containers.UnpauseContainerAsync(id);
                    break;
                case ControlContainerType.Logs:
                    {
                        ContainerLogsEvent Data = @event.Data.ToObject<ContainerLogsEvent>();
                        DockerContainerLogs Logs = new DockerContainerLogs();
                        Logs.ContainerName = Program.ContainerCache.GetValueOrDefault(id, id);

                        var Stream = await Program.DockerClient.Containers.GetContainerLogsAsync(id, false, new ContainerLogsParameters
                        {
                            Tail = Data.Limit.ToString(),
                            Timestamps = Data.ShowTimestamp,
                            ShowStdout = true,
                            ShowStderr = true,
                            Since = Data.SinceDate.HasValue ? ((DateTimeOffset)Data.SinceDate.Value).ToUnixTimeSeconds().ToString() : null,
                            Until = Data.UntilDate.HasValue ? ((DateTimeOffset)Data.UntilDate.Value).ToUnixTimeSeconds().ToString() : null,
                        }, CancellationToken.None);

                        (string stdout, string stderr) DataStream = await Stream.ReadOutputToEndAsync(CancellationToken.None);
                        Logs.SinceDate = DateTime.UtcNow;
                        Logs.Logs = DataStream.stdout;

                        return Logs;
                    }
                case ControlContainerType.Processes:
                    {
                        try
                        {
                            var Response = await Program.DockerClient.Containers.ListProcessesAsync(id, new ContainerListProcessesParameters
                            {
                                PsArgs = "-eo user,pid,ppid,thcount,c,%cpu,%mem,lstart,etime,comm,cmd --date-format %Y-%m-%dT%H:%M:%S"
                            });
                            return new DockerContainerProcesses
                            {
                                ContainerName = Program.ContainerCache.GetValueOrDefault(id, id),
                                Data = Response
                            };
                        }
                        catch (DockerApiException ex) when (ex.Message.Contains("is not running"))
                        {
                            return new DockerContainerProcesses
                            {
                                ContainerName = Program.ContainerCache.GetValueOrDefault(id, id),
                                IsNotRunning = true
                            };

                        }

                    }
            }

            return null;
        }
    }
}
