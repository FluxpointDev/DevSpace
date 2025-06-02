using DevSpaceShared.WebSocket;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceShared;
public class AgentStatsResponse : IWebSocketEvent
{
    public AgentStatsResponse() : base(EventType.GetAgentStats)
    {

    }

    public static async Task<AgentStatsResponse> Create(string? agentVersion, DockerClient client, SystemInfoResponse systemInfo)
    {
        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        VolumesListResponse Volumes = await client.Volumes.ListAsync();
        IList<NetworkResponse> Networks = await client.Networks.ListNetworksAsync();

        return new AgentStatsResponse
        {
            StacksCount = Containers.Count(x => x.Labels != null && x.Labels.TryGetValue("com.docker.compose.project", out string label) && !string.IsNullOrEmpty(label)),
            ContainersCount = systemInfo.Containers,
            ContainersRunningCount = systemInfo.ContainersRunning,
            ContainersDownCount = systemInfo.ContainersStopped,
            ContainersUnhealthyCount = Containers.Count(x => x.Status.Contains("unhealthy")),
            VolumesCount = Volumes.Volumes.Count(),
            NetworksCount = Networks.Count(),
            CpuCount = systemInfo.NCPU,
            TotalMemory = systemInfo.MemTotal,
            ImagesCount = systemInfo.Images,
            AgentVersion = agentVersion,
            DockerVersion = systemInfo.ServerVersion,
            IsWindows = systemInfo.OperatingSystem == "Docker Desktop"
        };
    }
    public long StacksCount { get; set; }
    public long ContainersCount { get; set; }
    public long ContainersRunningCount { get; set; }
    public long ContainersUnhealthyCount { get; set; }
    public long ContainersDownCount { get; set; }
    public long VolumesCount { get; set; }
    public long ImagesCount { get; set; }
    public long NetworksCount { get; set; }
    public long CpuCount { get; set; }
    public long TotalMemory { get; set; }
    public string? AgentVersion { get; set; }
    public string DockerVersion { get; set; }
    public bool IsWindows { get; set; }
}
