using DevSpaceShared.Responses;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerSystem
    {
        public static async Task<DevSpaceShared.Responses.SystemInfoResponse> GetSystemInfoAsync(DockerClient client)
        {
            var Info = await client.System.GetSystemInfoAsync();
            var Volumes = await client.Volumes.ListAsync();
            var Networks = await client.Networks.ListNetworksAsync();
            var Plugins = await client.Plugin.ListPluginsAsync(new PluginListParameters
            {

            });
            TimeSpan runtime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            List<string> Times = new List<string>();
            if (runtime.Days != 0)
                Times.Add($"{runtime.Days} Days {runtime.Hours} Hours");
            else if (runtime.Hours != 0)
                Times.Add($"{runtime.Hours} Hours {runtime.Minutes} Minutes");
            else if (runtime.Minutes != 0)
                Times.Add($"{runtime.Minutes} Minutes {runtime.Seconds} Seconds");
            else
                Times.Add($"{runtime.Seconds} Seconds");
            Console.WriteLine("Time: " + string.Join(" ", Times));
            return new DevSpaceShared.Responses.SystemInfoResponse
            {
                Docker = new SystemDockerInfo
                {
                    AllContainers = Info.Containers,
                    RunningContainers = Info.ContainersRunning,
                    StoppedContainers = Info.ContainersStopped,
                    PausedContainers = Info.ContainersPaused,
                    Images = Info.Images,
                    DockerVersion = Info.ServerVersion,
                    Volumes = Volumes.Volumes.Count(),
                    Networks = Networks.Count(),
                    Plugins = Plugins.Count()
                },
                Uptime = string.Join(" ", Times),
                OperatingSystem = Info.OperatingSystem,
                CpuCount = Info.NCPU,
                RamTotal = Info.MemTotal,
                KernelVersion = Info.KernelVersion,
                AgentNET = Environment.Version.ToString(),
                AgentVersion = Program.Version
            };
        }
    }
}
