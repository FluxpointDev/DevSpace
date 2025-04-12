using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace DevSpaceAgent.Docker;

public static class DockerStacks
{
    public static async Task<List<DockerStackInfo>> ListStacksAsync(DockerClient client)
    {
        List<DockerStackInfo> Stacks = new List<DockerStackInfo>();
        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
        {
            Size = true,
            All = true
        });

        foreach (ContainerListResponse? c in containers)
        {
            if (c.Labels == null || !c.Labels.TryGetValue("com.docker.compose.project", out string? label))
                continue;

            bool IsRunning = false;
            switch (c.State)
            {
                case "running":
                case "restarting":
                    IsRunning = true;
                    break;
            }

            DockerStackInfo? Stack = Stacks.FirstOrDefault(x => x.Name == label);
            if (Stack != null)
            {
                if (IsRunning)
                    Stack.IsRunning = true;
                Stack.ContainersCount += 1;
                if (c.Names != null && c.Names.Any())
                    Stack.Containers.Add(new DockerContainerInfo
                    {
                        Id = c.ID,
                        Name = c.Names.First().Substring(1)
                    });
                else
                    Stack.Containers.Add(new DockerContainerInfo
                    {
                        Id = c.ID,
                        Name = c.ID
                    });
            }
            else
            {
                string Id = "";
                DateTime? CreatedAt = c.Created;

                long Version = 0;
                DockerStackControl Type = DockerStackControl.System;
                string? DataPath = null;
                string? configFile = null;
                DateTime? UpdatedAt = null;

                if (c.Labels != null && c.Labels.TryGetValue("com.docker.compose.project.config_files", out configFile))
                    DataPath = configFile;

                if (string.IsNullOrEmpty(DataPath) && c.Labels != null && c.Labels.TryGetValue("com.docker.compose.project.working_dir", out string? workDir))
                    DataPath = workDir;


                if (!string.IsNullOrEmpty(DataPath) && DataPath.StartsWith("/data/compose"))
                {
                    Type = DockerStackControl.Portainer;
                    try
                    {
                        Id = DataPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[2];
                    }
                    catch { }

                    try
                    {
                        string Version2 = DataPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[3];
                        Version = long.Parse(Version2.Substring(1));
                    }
                    catch { }

                    if (Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/" + Id))
                    {
                        CreatedAt = Directory.GetCreationTimeUtc("/var/lib/docker/volumes/portainer_data/_data/compose/" + Id);
                        if (Version != 1 && Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/" + Id + "/v" + Version))
                            UpdatedAt = Directory.GetLastWriteTimeUtc("/var/lib/docker/volumes/portainer_data/_data/compose/" + Id + "/v" + Version);
                    }
                }

                if (Type == DockerStackControl.System && !string.IsNullOrEmpty(configFile) && configFile.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
                {
                    CreatedAt = c.Created;

                }

                if (!string.IsNullOrEmpty(DataPath) && DataPath.StartsWith("/app/Data/Stacks/"))
                {
                    Type = DockerStackControl.Full;

                    Id = DataPath.Split('/')[4];

                    if (Program.Stacks.TryGetValue(Id, out Data.StackFile? stack))
                    {
                        label = stack.Name;
                        Version = stack.Version;
                        CreatedAt = stack.CreatedAt;
                        UpdatedAt = stack.UpdatedAt;
                    }
                    else
                    {
                        try
                        {
                            CreatedAt = Directory.GetCreationTimeUtc(DataPath);
                        }
                        catch { }
                        try
                        {
                            UpdatedAt = File.GetCreationTimeUtc(configFile);
                        }
                        catch { }
                    }


                }


                Stacks.Add(new DockerStackInfo
                {
                    Id = Id,
                    Name = label,
                    CreatedAt = CreatedAt,
                    ContainersCount = 1,
                    Containers = new HashSet<DockerContainerInfo>
                        {
                            new DockerContainerInfo
                            {
                                Id = c.ID,
                                Name = (c.Names != null && c.Names.Any()) ? c.Names.First().Substring(1) : c.ID
                            }
                        },
                    Version = Version,
                    IsRunning = IsRunning,
                    ControlType = Type,
                    UpdatedAt = UpdatedAt
                });
            }
        }

        foreach (KeyValuePair<string, Data.StackFile> i in Program.Stacks)
        {
            DockerStackInfo? Stack = Stacks.FirstOrDefault(x => x.Name == i.Value.Name);
            if (Stack == null)
                Stacks.Add(new DockerStackInfo
                {
                    ControlType = DockerStackControl.Full,
                    Id = i.Key,
                    Containers = new HashSet<DockerContainerInfo>(),
                    Name = i.Value.Name,
                    CreatedAt = i.Value.CreatedAt,
                    UpdatedAt = i.Value.UpdatedAt,
                });
        }
        return Stacks;
    }

    public static async Task<DockerStackCreate> CreateStackAsync(DockerClient client, CreateStackEvent? compose)
    {
        if (compose == null)
            throw new Exception("Failed to parse stack creation options.");

        if (string.IsNullOrEmpty(compose.Name))
            throw new Exception("Stack needs a name.");

        if (Program.Stacks.Any(x => !string.IsNullOrEmpty(x.Value.Name) && x.Value.Name.Equals(compose.Name, StringComparison.OrdinalIgnoreCase)))
            throw new Exception("Stack name already exists.");

        string Id = Guid.NewGuid().ToString().Replace("-", "");
        string Dir = Program.CurrentDirectory + $"Data/Stacks/{Id}/";
        if (Directory.Exists(Dir))
            throw new Exception("Stack directory already exists.");

        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(Dir + "docker-compose.yml", compose.Content);
        }
        catch
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch { }
            throw new Exception("Failed to setup compose files.");
        }

        try
        {
            using (ICompositeService build = new Builder()
                .UseContainer()
                .UseCompose()
                .ServiceName(compose.Name)
                .KeepContainer()
                .KeepVolumes()
                .KeepOnDispose()
                .FromFile(Dir + "docker-compose.yml")
                .Build())
            {

            }
            ;
        }
        catch
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch { }


            throw;
        }

        Program.Stacks.Add(Id, new Data.StackFile
        {
            Id = Id,
            Name = compose.Name,
            CreatedAt = DateTime.UtcNow
        });
        Program.SaveStacks();
        return new DockerStackCreate
        {
            Id = Id
        };
    }

    public static async Task<List<DockerStackInfo>> ListPortainerStacks(DockerClient client)
    {
        List<DockerStackInfo> Stacks = new List<DockerStackInfo>();
        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
        {
            All = true
        });
        foreach (ContainerListResponse i in containers)
        {
            if (i.Labels != null)
            {
                string? Directory = null;
                if (i.Labels.TryGetValue("com.docker.compose.project.config_files", out string? configFile) && !string.IsNullOrEmpty(configFile))
                    Directory = configFile;
                else if (i.Labels.TryGetValue("com.docker.compose.project.working_dir", out string? workDir) && !string.IsNullOrEmpty(workDir))
                    Directory = workDir;

                if (!i.Labels.TryGetValue("com.docker.compose.project", out string? label))
                    continue;

                if (!string.IsNullOrEmpty(Directory) && !string.IsNullOrEmpty(label) && Directory.StartsWith("/data/compose/"))
                {
                    Stacks.Add(new DockerStackInfo
                    {
                        Id = Directory.Split('/', StringSplitOptions.RemoveEmptyEntries)[2],
                        Name = label
                    });
                }
            }
        }

        return Stacks;
    }

    private static void CloneDirectory(string root, string dest)
    {
        foreach (string directory in Directory.GetDirectories(root))
        {
            //Get the path of the new directory
            string newDirectory = Path.Combine(dest, Path.GetFileName(directory));
            //Create the directory if it doesn't already exist
            Directory.CreateDirectory(newDirectory);
            //Recursively clone the directory
            CloneDirectory(directory, newDirectory);
        }

        foreach (string file in Directory.GetFiles(root))
        {
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
        }
    }

    public static async Task<DockerStackCreate> ImportPortainerStack(DockerClient client, DockerStackInfo? stack)
    {
        if (stack == null)
            throw new Exception("Stack arguments failed to parse.");

        if (!Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/"))
            throw new Exception("Dev Space Agent has not been mounted with the Portainer compose folder to import containers.");

        if (!Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/" + stack.Id))
            throw new Exception("Stack folder does not exist.");

        int Version = -1;

        foreach (string d in Directory.GetDirectories("/var/lib/docker/volumes/portainer_data/_data/compose/" + stack.Id))
        {
            string Split = d.Split('/').Last();
            if (Split.StartsWith("v") && int.TryParse(Split.Substring(1), out int stackNumber) && stackNumber > Version)
                Version = stackNumber;
        }

        if (Version == -1)
            throw new Exception("Stack version folder does not exist.");

        string ComposeContent = File.ReadAllText($"/var/lib/docker/volumes/portainer_data/_data/compose/{stack.Id}/v{Version}/docker-compose.yml");

        if (string.IsNullOrEmpty(ComposeContent))
            throw new Exception("Failed to parse stack compose.");

        // Create stack for import
        if (Program.Stacks.Any(x => !string.IsNullOrEmpty(x.Value.Name) && x.Value.Name.Equals(stack.Name, StringComparison.OrdinalIgnoreCase)))
            throw new Exception("Stack name already exists.");

        string Id = Guid.NewGuid().ToString().Replace("-", "");
        string Dir = Program.CurrentDirectory + $"Data/Stacks/{Id}/";
        if (Directory.Exists(Dir))
            throw new Exception("Stack directory already exists.");

        try
        {
            Directory.CreateDirectory(Dir);
            CloneDirectory($"/var/lib/docker/volumes/portainer_data/_data/compose/{stack.Id}/v{Version}/", Dir);
        }
        catch
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch { }
            throw new Exception("Failed to setup compose files.");
        }

        try
        {
            IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                { { "label", new Dictionary<string, bool> { { "com.docker.compose.project=" + stack.Name, true } } } }
            });
            foreach (ContainerListResponse i in Containers)
            {
                await client.Containers.RemoveContainerAsync(i.ID, new ContainerRemoveParameters
                {
                    Force = true
                });
            }
        }
        catch { }

        try
        {
            using (ICompositeService build = new Builder()
                .UseContainer()
                .UseCompose()
                .ServiceName(stack.Name)
                .KeepContainer()
                .KeepVolumes()
                .KeepOnDispose()
                .FromFile(Dir + "docker-compose.yml")
                .Build())
            {
                try
                {
                    Console.WriteLine("Starting...");
                    build.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Runtime Failed");
                    Console.WriteLine(ex);
                }
            }
           ;
        }
        catch
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch { }
            throw;
        }

        Program.Stacks.Add(Id, new Data.StackFile
        {
            Id = Id,
            Name = stack.Name,
            CreatedAt = DateTime.UtcNow
        });
        Program.SaveStacks();
        return new DockerStackCreate
        {
            Id = Id
        };
    }

    public static async Task<DockerStackCreate> RecreateContainer(DockerClient client, string id, CreateStackEvent create)
    {
        if (!Program.Stacks.TryGetValue(id, out Data.StackFile? stack))
            throw new Exception("Stack does not exist.");


        string Dir = Program.CurrentDirectory + $"Data/Stacks/{id}/";
        if (!Directory.Exists(Dir))
            Directory.CreateDirectory(Dir);


        string File = Dir + "docker-compose.yml";

        string CurrentData = System.IO.File.ReadAllText(File);

        try
        {
            System.IO.File.WriteAllText(File, create.Content);
        }
        catch
        {
            throw new Exception("Failed to setup compose files.");
        }

        bool IsSuccess = false;
        ServiceRunningState? CurrentState = null;
        try
        {
            using (DockerComposeCompositeService svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { File },
                ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
                StopOnDispose = false,
                KeepContainers = true,
                RemoveOrphans = true,
                AlternativeServiceName = stack.Name,
                KeepVolumes = true
            }))
            {
                CurrentState = svc.State;
                svc.Stop();
                svc.Remove();
                svc.Start();
                IsSuccess = true;
            }
            ;
        }
        catch
        {
            try
            {
                System.IO.File.WriteAllText(File, CurrentData);
            }
            catch { }

            if (CurrentState.HasValue && CurrentState.Value == ServiceRunningState.Running)
            {
                try
                {
                    using (DockerComposeCompositeService svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
                    {
                        ComposeFilePath = new List<string> { File },
                        ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
                        StopOnDispose = false,
                        AlternativeServiceName = stack.Name,
                        KeepContainers = true,
                        KeepVolumes = true
                    }))
                    {
                        svc.Start();
                    }
                }
                catch { }
            }

            throw;
        }
        if (!IsSuccess)
        {
            try
            {
                System.IO.File.WriteAllText(File, CurrentData);
            }
            catch { }

            if (CurrentState.HasValue && CurrentState.Value == ServiceRunningState.Running)
            {
                try
                {
                    using (DockerComposeCompositeService svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
                    {
                        ComposeFilePath = new List<string> { File },
                        ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
                        StopOnDispose = false,
                        AlternativeServiceName = stack.Name,
                        KeepContainers = true,
                        KeepVolumes = true
                    }))
                    {
                        svc.Start();
                    }
                }
                catch { }
            }

            throw new Exception("Failed to create stack.");
        }

        stack.Version += 1;
        stack.UpdatedAt = DateTime.UtcNow;
        Program.SaveStacks();

        return new DockerStackCreate
        {
            Id = id
        };
    }

    public static async Task<DockerStackInfo> ViewStack(DockerClient client, string id)
    {
        if (Program.Stacks.TryGetValue(id, out Data.StackFile? stack))
        {
            DockerStackInfo Info = new DockerStackInfo
            {
                Id = stack.Id,
                ControlType = DockerStackControl.Full,
                CreatedAt = stack.CreatedAt,
                Name = stack.Name,
                UpdatedAt = stack.UpdatedAt,
                Version = stack.Version
            };


            try
            {
                IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                { { "label", new Dictionary<string, bool> { { "com.docker.compose.project=" + stack.Name, true } } } }
                });

                foreach (ContainerListResponse? c in Containers)
                {
                    Info.ContainersCount += 1;
                    Info.Containers.Add(new DockerContainerInfo
                    {
                        Id = c.ID,
                        Name = c.Names != null && c.Names.Any() ? c.Names.First().Substring(1) : c.ID,
                        State = c.State,
                        Status = c.Status,
                        CreatedAt = c.Created,
                        ImageId = c.ImageID,
                        ImageName = c.Image
                    });

                    foreach (KeyValuePair<string, EndpointSettings> i in c.NetworkSettings.Networks)
                    {
                        if (!Info.Networks.Any(x => x.Id != i.Value.NetworkID))
                        {
                            Info.Networks.Add(DockerContainerNetwork.Create(i.Key, i.Value));
                        }
                    }

                    foreach (MountPoint? i in c.Mounts)
                    {
                        if (!Info.Volumes.Any(x => x.Name == i.Name))
                        {
                            Info.Volumes.Add(i);
                        }
                    }
                }

            }
            catch { }
            return Info;
        }
        else
        {
            DockerStackInfo Info = new DockerStackInfo
            {
                Id = id,
                ControlType = DockerStackControl.System,
            };


            if (int.TryParse(id, out _))
            {
                Info.ControlType = DockerStackControl.Portainer;
                IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true
                });

                foreach (ContainerListResponse? c in Containers)
                {
                    string? DataPath = null;
                    if (c.Labels != null)
                    {
                        if (c.Labels.TryGetValue("com.docker.compose.project.config_files", out string confFile))
                            DataPath = confFile;
                        if (string.IsNullOrEmpty(DataPath) && c.Labels.TryGetValue("com.docker.compose.project.working_dir", out string workDir))
                            DataPath = workDir;
                    }

                    if (string.IsNullOrEmpty(DataPath) || !DataPath.StartsWith("/data/compose/" + id))
                        continue;

                    if (string.IsNullOrEmpty(Info.Name) && c.Labels != null && c.Labels.TryGetValue("com.docker.compose.project", out string name))
                        Info.Name = name;

                    if (Info.Version == 0)
                    {
                        try
                        {
                            string Version = DataPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Substring(1);
                            if (long.TryParse(Version, out long version))
                                Info.Version = version;
                        }
                        catch { }
                    }


                    if (Info.CreatedAt == null && Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/" + id))
                    {
                        Info.CreatedAt = Directory.GetCreationTimeUtc("/var/lib/docker/volumes/portainer_data/_data/compose/" + id);
                        if (Info.Version != 1 && Directory.Exists("/var/lib/docker/volumes/portainer_data/_data/compose/" + id + "/v" + Info.Version))
                            Info.UpdatedAt = Directory.GetLastWriteTimeUtc("/var/lib/docker/volumes/portainer_data/_data/compose/" + id + "/v" + Info.Version);
                    }

                    switch (c.State)
                    {
                        case "running":
                        case "restarting":
                            Info.IsRunning = true;
                            break;
                    }

                    Info.ContainersCount += 1;
                    Info.Containers.Add(new DockerContainerInfo
                    {
                        Id = c.ID,
                        Name = c.Names != null && c.Names.Any() ? c.Names.First().Substring(1) : c.ID,
                        State = c.State,
                        Status = c.Status,
                        CreatedAt = c.Created,
                        ImageId = c.ImageID,
                        ImageName = c.Image
                    });

                    foreach (KeyValuePair<string, EndpointSettings> i in c.NetworkSettings.Networks)
                    {
                        if (!Info.Networks.Any(x => x.Id != i.Value.NetworkID))
                        {
                            Info.Networks.Add(DockerContainerNetwork.Create(i.Key, i.Value));
                        }
                    }

                    foreach (MountPoint? i in c.Mounts)
                    {
                        if (!Info.Volumes.Any(x => x.Name == i.Name))
                        {
                            Info.Volumes.Add(i);
                        }
                    }
                }
            }
            else
            {
                IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                        { { "label", new Dictionary<string, bool> { { $"com.docker.compose.project={id}", true } } } }
                });

                foreach (ContainerListResponse? c in Containers)
                {
                    if (string.IsNullOrEmpty(Info.Name) && c.Labels != null && c.Labels.TryGetValue("com.docker.compose.project", out string? name))
                        Info.Name = name;

                    if (Info.CreatedAt == null || c.Created < Info.CreatedAt)
                        Info.CreatedAt = c.Created;

                    switch (c.State)
                    {
                        case "running":
                        case "restarting":
                            Info.IsRunning = true;
                            break;
                    }

                    Info.ContainersCount += 1;
                    Info.Containers.Add(new DockerContainerInfo
                    {
                        Id = c.ID,
                        Name = c.Names != null && c.Names.Any() ? c.Names.First().Substring(1) : c.ID,
                        State = c.State,
                        Status = c.Status,
                        CreatedAt = c.Created,
                        ImageId = c.ImageID,
                        ImageName = c.Image,
                    });

                    foreach (KeyValuePair<string, EndpointSettings> i in c.NetworkSettings.Networks)
                    {
                        if (!Info.Networks.Any(x => x.Id != i.Value.NetworkID))
                        {
                            Info.Networks.Add(DockerContainerNetwork.Create(i.Key, i.Value));
                        }
                    }

                    foreach (MountPoint? i in c.Mounts)
                    {
                        if (!Info.Volumes.Any(x => x.Name == i.Name))
                        {
                            Info.Volumes.Add(i);
                        }
                    }
                }
            }

            return Info;
        }
    }

    public static async Task<DockerStackComposeInfo> StackCompose(DockerClient client, string id)
    {
        if (!Program.Stacks.TryGetValue(id, out Data.StackFile? stack))
            throw new Exception("Stack does not exist.");

        if (!File.Exists(Program.CurrentDirectory + $"Data/Stacks/{id}/docker-compose.yml"))
            return new DockerStackComposeInfo
            {
                Name = stack.Name
            };

        try
        {
            return new DockerStackComposeInfo
            {
                Name = stack.Name,
                Content = File.ReadAllText(Program.CurrentDirectory + $"Data/Stacks/{id}/docker-compose.yml")
            };
        }
        catch
        {
            throw new Exception("Failed to get compose content.");
        }
    }

    public static async Task ControlStack(DockerClient client, string id, ControlStackType type)
    {
        if (!Program.Stacks.TryGetValue(id, out Data.StackFile? stack))
            throw new Exception("Stack does not exist.");

        string Dir = Program.CurrentDirectory + $"Data/Stacks/{id}/";
        if (!Directory.Exists(Dir))
            throw new Exception("This stack does not exist anymore.");
        string File = Dir + "docker-compose.yml";
        using (DockerComposeCompositeService svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
        {
            ComposeFilePath = new List<string> { File },
            ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
            StopOnDispose = false,
            AlternativeServiceName = stack.Name,
            KeepContainers = true,
            KeepVolumes = true
        }))
        {
            switch (type)
            {
                case ControlStackType.Start:
                case ControlStackType.Resume:
                    svc.Start();
                    break;
                case ControlStackType.Stop:
                    svc.Stop();
                    break;
                case ControlStackType.Pause:
                    svc.Pause();
                    break;
                case ControlStackType.Restart:
                    svc.Stop();
                    svc.Start();
                    break;
            }
        }
    }

    public static async Task RemoveStack(DockerClient client, string id)
    {
        string Dir = Program.CurrentDirectory + $"Data/Stacks/{id}/";
        if (Directory.Exists(Dir))
        {
            string File = Dir + "docker-compose.yml";
            if (System.IO.File.Exists(File))
            {
                try
                {
                    using (DockerComposeCompositeService svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
                    {
                        ComposeFilePath = new List<string> { File },
                        ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
                        StopOnDispose = false,
                        RemoveOrphans = true,
                        KeepContainers = false,
                        KeepVolumes = true
                    }))
                    {
                        foreach (IContainerService? i in svc.Containers)
                        {
                            i.Remove(true);
                        }
                        svc.Remove(true);
                    }
                }
                catch { }
            }
            try
            {
                Directory.Delete(Dir, true);
            }
            catch { }
        }

        if (Program.Stacks.ContainsKey(id))
        {
            Program.Stacks.Remove(id);
            Program.SaveStacks();
        }
    }
}
