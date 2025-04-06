using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerContainerProcesses
{
    public string? ContainerName { get; set; }
    public ContainerProcessesResponse? Data { get; set; }

    public DockerContainerStats? Stats { get; set; }
    public bool IsNotRunning { get; set; }
}
public class DockerContainerStats
{
    public static DockerContainerStats Create(DockerStatJson json)
    {
        DockerContainerStats Stats = new DockerContainerStats
        {
            MemoryUsage = json.memory_stats.usage - json.memory_stats.stats.cache,
            Date = DateTime.UtcNow
        };
        if (json.networks.Any())
        {
            Stats.NetworkReceive = json.networks.First().Value.rx_bytes;
            Stats.NetworkTransmit = json.networks.First().Value.tx_bytes;
        }

        return Stats;
    }

    public long MemoryUsage { get; set; }
    public float CpuUsage { get; set; }
    public long IORead { get; set; }
    public long IOWrite { get; set; }
    public long NetworkTransmit { get; set; }
    public long NetworkReceive { get; set; }
    public DateTime Date { get; set; }
}