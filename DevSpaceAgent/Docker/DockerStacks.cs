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
            if (!c.Labels.TryGetValue("com.docker.compose.project", out string label))
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

                if (string.IsNullOrEmpty(DataPath) && c.Labels != null && c.Labels.TryGetValue("com.docker.compose.project.working_dir", out string workDir))
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
                }

                if (Type == DockerStackControl.System && !string.IsNullOrEmpty(configFile) && configFile.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
                {
                    CreatedAt = null;
                    UpdatedAt = c.Created;
                }

                if (!string.IsNullOrEmpty(DataPath) && DataPath.StartsWith("/app/Data/Stacks/"))
                {
                    Type = DockerStackControl.Full;

                    Id = DataPath.Split('/')[4];

                    if (Program.Stacks.TryGetValue(Id, out var stack))
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

        foreach (var i in Program.Stacks)
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

    public static async Task<DockerStackCreate> CreateStack(DockerClient client, CreateStackEvent compose)
    {
        if (string.IsNullOrEmpty(compose.Name))
            throw new Exception("Stack needs a name.");

        if (Program.Stacks.Any(x => x.Value.Name.Equals(compose.Name, StringComparison.OrdinalIgnoreCase)))
            throw new Exception("Stack name already exists.");

        Console.WriteLine("Create Stack");
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
            throw new Exception("Failed to setup compose files.");
        }

        try
        {
            using (var build = new Builder()
                .UseContainer()
                .UseCompose()
                .ServiceName(compose.Name)
                .KeepContainer()
                .KeepVolumes()
                .KeepOnDispose()
                .FromFile(Dir + "docker-compose.yml")
                .Build()) { }
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

    public static async Task<DockerStackCreate> RecreateContainer(DockerClient client, string id, CreateStackEvent create)
    {
        if (!Program.Stacks.TryGetValue(id, out var stack))
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
            using (var svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
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
                    using (var svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
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
                    using (var svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
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
        if (Program.Stacks.TryGetValue(id, out var stack))
        {
            var Info = new DockerStackInfo
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
                var Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                { { "label", new Dictionary<string, bool> { { "com.docker.compose.project=" + stack.Name, true } } } }
                });

                foreach (var c in Containers)
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
                }

            }
            catch { }
            return Info;
        }

        return new DockerStackInfo
        {
            ControlType = DockerStackControl.System
        };
    }

    public static async Task<DockerStackComposeInfo> StackCompose(DockerClient client, string id)
    {
        if (!Program.Stacks.TryGetValue(id, out var stack))
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
        if (!Program.Stacks.TryGetValue(id, out var stack))
            throw new Exception("Stack does not exist.");

        Console.WriteLine($"{type.ToString()} Stack");
        string Dir = Program.CurrentDirectory + $"Data/Stacks/{id}/";
        if (!Directory.Exists(Dir))
            throw new Exception("This stack does not exist anymore.");
        string File = Dir + "docker-compose.yml";
        using (var svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
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

            Console.WriteLine("State: " + svc.State.ToString());
        }
    }

    public static async Task RemoveStack(DockerClient client, string id)
    {
        Console.WriteLine("Remove Stack");
        string Dir = Program.CurrentDirectory + $"Data/Stacks/{id}/";
        if (Directory.Exists(Dir))
        {
            string File = Dir + "docker-compose.yml";
            if (System.IO.File.Exists(File))
            {
                try
                {
                    using (var svc = new DockerComposeCompositeService(new Hosts().Discover().FirstOrDefault(), new DockerComposeConfig
                    {
                        ComposeFilePath = new List<string> { File },
                        ImageRemoval = Ductus.FluentDocker.Model.Images.ImageRemovalOption.None,
                        StopOnDispose = false,
                        KeepContainers = false,
                        KeepVolumes = true
                    }))
                    {
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
