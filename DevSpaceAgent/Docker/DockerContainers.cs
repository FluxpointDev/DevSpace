using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text;

namespace DevSpaceAgent.Docker;

public static class DockerContainers
{
    public static async Task<IList<DockerContainerInfo>> ListContainersAsync(DockerClient client)
    {
        try
        {
            IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
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

    public static async Task<ContainerListResponse?> GetContainerAsync(DockerClient client, string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Container id is missing.");

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
        CreateContainerParameters? Data = @event.Data?.ToObject<CreateContainerParameters>();
        if (Data == null)
            throw new Exception("Failed to parse container creation options.");

        return await client.Containers.CreateContainerAsync(Data);
    }

    public static async Task<ContainerUpdateResponse> UpdateContainerAsync(DockerClient client, DockerEvent @event)
    {
        ContainerUpdateParameters? Data = @event.Data?.ToObject<ContainerUpdateParameters>();
        if (Data == null)
            throw new Exception("Failed to parse container update options.");

        return await client.Containers.UpdateContainerAsync(@event.ResourceId, Data);
    }

    public static async Task<CreateContainerResponse?> RecreateContainerAsync(DockerClient client, DockerEvent @event)
    {
        ContainerInspectResponse? Info = await client.Containers.InspectContainerAsync(@event.ResourceId);
        if (Info == null)
            throw new Exception("Failed to get container info.");


        bool ContainerRunning = false;
        bool RenameDone = false;
        try
        {

            await client.Containers.StopContainerAsync(@event.ResourceId, new ContainerStopParameters
            {

            });
            ContainerRunning = Info.State.Running;

            await client.Containers.RenameContainerAsync(@event.ResourceId, new ContainerRenameParameters
            {
                NewName = Info.Name + "-old"
            }, CancellationToken.None);
            RenameDone = true;

            foreach (var i in Info.NetworkSettings.Networks)
            {
                await client.Networks.DisconnectNetworkAsync(i.Value.NetworkID, new NetworkDisconnectParameters
                {
                    Container = @event.ResourceId,
                    Force = true
                });
            }



            CreateContainerParameters Config = new CreateContainerParameters(Info.Config)
            {
                Name = Info.Name,
                Platform = Info.Platform,
                HostConfig = null,
                NetworkingConfig = null
            };
            CreateContainerResponse CreateContainer = await client.Containers.CreateContainerAsync(Config);

            foreach (var i in Info.NetworkSettings.Networks)
            {
                _ = client.Networks.ConnectNetworkAsync(i.Value.NetworkID, new NetworkConnectParameters
                {
                    Container = CreateContainer.ID,
                    EndpointConfig = i.Value
                });
            }

            await client.Containers.StartContainerAsync(CreateContainer.ID, new ContainerStartParameters
            {

            });

            try
            {
                await client.Containers.StopContainerAsync(@event.ResourceId, new ContainerStopParameters
                {

                });
            }
            catch { }

            try
            {
                await client.Containers.RemoveContainerAsync(@event.ResourceId, new ContainerRemoveParameters
                {
                    Force = true
                });
            }
            catch { }

            return CreateContainer;
        }
        catch (Exception ex)
        {
            if (RenameDone)
            {
                _ = client.Containers.RenameContainerAsync(@event.ResourceId, new ContainerRenameParameters
                {
                    NewName = Info.Name
                }, CancellationToken.None);
            }

            foreach (var i in Info.NetworkSettings.Networks)
            {
                _ = client.Networks.ConnectNetworkAsync(i.Value.NetworkID, new NetworkConnectParameters
                {
                    Container = @event.ResourceId,
                    EndpointConfig = i.Value
                });
            }

            if (ContainerRunning)
            {
                _ = client.Containers.StartContainerAsync(@event.ResourceId, new ContainerStartParameters
                {

                });
            }

            throw new Exception("Failed to recreate container, " + ex.Message);
        }
    }

    public static async Task<object?> ControlContainerAsync(DockerClient client, DockerEvent @event, string id)
    {
        switch (@event.ContainerType)
        {
            case ControlContainerType.Recreate:
                return await RecreateContainerAsync(client, @event);
            case ControlContainerType.View:
                return await GetContainerAsync(client, @event.ResourceId);
            case ControlContainerType.Update:
                return await UpdateContainerAsync(client, @event);
            case ControlContainerType.Inspect:
                return await client.Containers.InspectContainerAsync(id);
            case ControlContainerType.Changes:
                IList<ContainerFileSystemChangeResponse> Result = await client.Containers.InspectChangesAsync(id);
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
                    ContainerLogsEvent? Data = @event.Data?.ToObject<ContainerLogsEvent>();
                    if (Data == null)
                        throw new Exception("Failed to parse container log options.");

                    DockerContainerLogs Logs = new DockerContainerLogs();
                    Logs.ContainerName = Program.ContainerCache.GetValueOrDefault(id, id);

                    MultiplexedStream Stream = await client.Containers.GetContainerLogsAsync(id, false, new ContainerLogsParameters
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
                        ContainerProcessesResponse Response = await client.Containers.ListProcessesAsync(id, new ContainerListProcessesParameters
                        {
                            PsArgs = "-eo user,pid,ppid,thcount,c,%cpu,%mem,lstart,etime,comm,cmd --date-format %Y-%m-%dT%H:%M:%S"
                        });
                        DockerStatJson? Stats = null;
                        try
                        {
                            using (Stream StatsStream = await client.Containers.GetContainerStatsAsync(id, new ContainerStatsParameters
                            {
                                Stream = false
                            }, CancellationToken.None))
                            {
                                using (StreamReader reader = new StreamReader(StatsStream, Encoding.UTF8))
                                {
                                    string Json = reader.ReadToEnd();
                                    Stats = Newtonsoft.Json.JsonConvert.DeserializeObject<DockerStatJson>(Json);
                                }
                            }
                        }
                        catch { }

                        return new DockerContainerProcesses
                        {
                            ContainerName = Program.ContainerCache.GetValueOrDefault(id, id),
                            Data = Response,
                            Stats = Stats != null ? DockerContainerStats.Create(Stats) : null
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
            case ControlContainerType.Stats:
                {
                    DockerStatJson? Stats = null;
                    try
                    {
                        using (Stream StatsStream = await client.Containers.GetContainerStatsAsync(id, new ContainerStatsParameters
                        {
                            Stream = false
                        }, CancellationToken.None))
                        {
                            using (StreamReader reader = new StreamReader(StatsStream, Encoding.UTF8))
                            {
                                string Json = reader.ReadToEnd();
                                Stats = Newtonsoft.Json.JsonConvert.DeserializeObject<DockerStatJson>(Json);
                            }
                        }
                    }
                    catch { }
                    return Stats != null ? DockerContainerStats.Create(Stats) : null;
                }
            case ControlContainerType.Rename:
                {
                    CreateContainerEvent? Data = @event.Data?.ToObject<CreateContainerEvent>();
                    if (Data == null)
                        throw new Exception("Failed to parse container rename options.");

                    await client.Containers.RenameContainerAsync(@event.ResourceId, new ContainerRenameParameters
                    {
                        NewName = Data.Name
                    }, CancellationToken.None);
                }
                break;
            case ControlContainerType.Scan:
                {
                    var CurrentContainer = await client.Containers.InspectContainerAsync(id);
                    if (!CurrentContainer.Mounts.Any() || !CurrentContainer.Mounts.Any(x => x.Type == "bind" || x.Type == "volume"))
                        return new CreateContainerResponse()
                        {
                            ID = id,
                        };

                    await client.Images.CreateImageAsync(new ImagesCreateParameters
                    {
                        FromImage = "docker.io/aquasec/trivy",
                        Tag = "latest"
                    }, null, new Progress<JSONMessage>());

                    var Binds = new List<string>
                    {
                        "/var/trivy:/root/.cache:rw"
                    };

                    foreach (var i in CurrentContainer.Mounts)
                    {
                        if (i.Type == "volume")
                        {
                            Binds.Add($"{i.Name}:/root/mount/{i.Name}:ro");
                        }
                        else if (i.Type == "bind")
                        {
                            Binds.Add($"{i.Source}:/root/mount{i.Source}:ro");
                        }
                    }

                    CreateContainerResponse Container = await client.Containers.CreateContainerAsync(new CreateContainerParameters
                    {
                        Cmd = new List<string>
                        {
                            "filesystem",
                            "/root/mount/",
                            "--skip-files",
                            "-q",
                            "--no-progress",
                            "--format",
                            "json",
                            "--scanners",
                            "vuln"
                        },
                        Image = "docker.io/aquasec/trivy",
                        Name = "security-scan-" + Guid.NewGuid().ToString().Replace("-", ""),
                        HostConfig = new HostConfig
                        {
                            Binds = Binds
                        }
                    });

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(new TimeSpan(0, 5, 0));
                        await client.Containers.RemoveContainerAsync(Container.ID, new ContainerRemoveParameters
                        {
                            Force = true,
                        });
                    });
                    await client.Containers.StartContainerAsync(Container.ID, new ContainerStartParameters
                    {

                    });

                    return new CreateContainerResponse()
                    {
                        ID = Container.ID,
                    };
                }
            case ControlContainerType.ScanReport:
                {
                    CreateContainerResponse? Container = @event.Data?.ToObject<CreateContainerResponse>();
                    if (Container == null)
                        throw new Exception("Failed to parse container scan options.");

                    var GetContainer = await client.Containers.InspectContainerAsync(Container.ID);
                    if (GetContainer == null || GetContainer.State.ExitCode != 0)
                        return new SecurityData { IsComplete = false };

                    MultiplexedStream Stream = await client.Containers.GetContainerLogsAsync(Container.ID, false, new ContainerLogsParameters
                    {
                        Tail = "all",
                        Timestamps = false,
                        ShowStdout = true,
                        ShowStderr = false
                    }, CancellationToken.None);

                    (string stdout, string stderr) DataStream = await Stream.ReadOutputToEndAsync(CancellationToken.None);

                    try
                    {
                        await client.Containers.RemoveContainerAsync(Container.ID, new ContainerRemoveParameters
                        {
                            Force = true
                        });
                    }
                    catch { }

                    return new SecurityData { IsComplete = true, Logs = DataStream.stdout };
                }
        }

        return null;
    }
}
