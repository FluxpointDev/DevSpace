using DevSpaceShared.Responses;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerSystem
    {
        private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }
            decimalPlaces = 0;
            if (mag > 2)
                decimalPlaces = 2;

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static async Task<SystemInfoFullResponse> GetHostInfoAsync(DockerClient client)
        {
            global::Docker.DotNet.Models.SystemInfoResponse Info = await client.System.GetSystemInfoAsync();
            var Version = await client.System.GetVersionAsync();
            var Drives = DriveInfo.GetDrives();
            DriveInfo CurrentDrive = Drives.First();
            //SystemInfoDriveResponse[] DriveList = Drives.Select(x => new SystemInfoDriveResponse
            //{
            //    Name = x.Name,
            //    Directory = x.RootDirectory.FullName,
            //    TotalSize = x.TotalSize,
            //    FreeSize = x.TotalFreeSpace
            //}).ToArray();
            return new DevSpaceShared.Responses.SystemInfoFullResponse
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
                //Drives = DriveList,
                StorageTotalSize = CurrentDrive.TotalSize,
                StorageFreeSize = CurrentDrive.TotalFreeSpace,
                RamTotal = Info.MemTotal,
                Hostname = Info.Name,
                StorageDriver = Info.Driver,
                LoggingDriver = Info.LoggingDriver,
                SystemDriver = Info.CgroupDriver,
                Uptime = Environment.TickCount64,
                Warnings = Info.Warnings != null ? Info.Warnings : new List<string>(),
                LogPlugins = Info.Plugins.Log != null ? Info.Plugins.Log : new List<string>(),
                NetworkPlugins = Info.Plugins.Network != null ? Info.Plugins.Network : new List<string>(),
                VolumePlugins = Info.Plugins.Volume != null ? Info.Plugins.Volume : new List<string>(),
                AuthPlugins = Info.Plugins.Authorization != null ? Info.Plugins.Authorization : new List<string>(),
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

            List<string> Stacks = new List<string>();

            foreach (ContainerListResponse? c in containers)
            {
                if (c.Labels.TryGetValue("com.docker.compose.project", out string? label))
                {
                    try
                    {
                        Stacks.Add(label!);
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
}
