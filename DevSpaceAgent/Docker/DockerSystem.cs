using DevSpaceShared.Responses;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker;

public static class DockerSystem
{
    public static async Task<SystemInfoFullResponse> GetHostInfoAsync(DockerClient client)
    {
        global::Docker.DotNet.Models.SystemInfoResponse Info = await client.System.GetSystemInfoAsync();
        VersionResponse Version = await client.System.GetVersionAsync();
        DriveInfo[] Drives = DriveInfo.GetDrives();
        DriveInfo CurrentDrive = Drives.First();
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Drives.First(), Newtonsoft.Json.Formatting.Indented));
        return new SystemInfoFullResponse
        {
            Docker = new SystemDockerInfo
            {
                DockerVersion = Info.ServerVersion,
                APIVersion = Version.APIVersion
            },
            OperatingSystem = Info.OperatingSystem,
            AgentNET = Environment.Version.ToString(),
            AgentVersion = Program.Version,
            KernelVersion = Info.KernelVersion,
            RootDirectory = Info.DockerRootDir,
            CpuCount = Info.NCPU,
            StorageTotalSize = CurrentDrive.TotalSize,
            StorageFreeSize = CurrentDrive.TotalFreeSpace,
            RamTotal = Info.MemTotal,
            Hostname = Info.Name,
            StorageDriver = Info.Driver,
            LoggingDriver = Info.LoggingDriver,
            SystemDriver = Info.CgroupDriver,
            Uptime = Environment.TickCount64,
            Warnings = Info.Warnings != null ? Info.Warnings : [],
            LogPlugins = Info.Plugins.Log != null ? Info.Plugins.Log : [],
            NetworkPlugins = Info.Plugins.Network != null ? Info.Plugins.Network : [],
            VolumePlugins = Info.Plugins.Volume != null ? Info.Plugins.Volume : [],
            AuthPlugins = Info.Plugins.Authorization != null ? Info.Plugins.Authorization : [],
        };
    }

    public static async Task<DevSpaceShared.Responses.SystemInfoResponse> GetSystemInfoAsync(DockerClient client)
    {
        global::Docker.DotNet.Models.SystemInfoResponse Info = await client.System.GetSystemInfoAsync();
        VolumesListResponse Volumes = await client.Volumes.ListAsync();
        IList<NetworkResponse> Networks = await client.Networks.ListNetworksAsync();
        IList<Plugin> Plugins = await client.Plugin.ListPluginsAsync(new PluginListParameters
        {

        });

        IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
        {
            Size = true,
            All = true
        });

        List<string> Stacks = [];

        foreach (ContainerListResponse? c in containers)
        {
            if (c.Labels.TryGetValue("com.docker.compose.project", out string? label) && !string.IsNullOrEmpty(label))
            {
                try
                {
                    Stacks.Add(label);
                }
                catch { }
            }
        }

        DriveInfo CurrentDrive = DriveInfo.GetDrives().First();
        return new DevSpaceShared.Responses.SystemInfoResponse
        {
            Docker = new SystemDockerInfo
            {
                AllContainers = Info.Containers,
                RunningContainers = Info.ContainersRunning,
                StoppedContainers = Info.ContainersStopped,
                Images = Info.Images,
                DockerVersion = Info.ServerVersion,
                Volumes = Volumes.Volumes.Count,
                Networks = Networks.Count,
                Plugins = Plugins.Count,
                Stacks = Stacks.Count
            },
            Uptime = Environment.TickCount64,
            OperatingSystem = Info.OperatingSystem,
            CpuCount = Info.NCPU,
            StorageTotalSize = CurrentDrive.TotalSize,
            StorageFreeSize = CurrentDrive.TotalFreeSpace,
            RamTotal = Info.MemTotal,
            AgentNET = Environment.Version.ToString(),
            AgentVersion = Program.Version
        };
    }
}
