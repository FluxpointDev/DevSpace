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
        long NumberCpus = json.cpu_stats.online_cpus;
        double CpuDelta = json.cpu_stats.cpu_usage.total_usage - json.precpu_stats.cpu_usage.total_usage;
        double SystemCpuDelta = json.cpu_stats.system_cpu_usage - json.precpu_stats.system_cpu_usage;
        double CpuUsage = ((CpuDelta / SystemCpuDelta) * NumberCpus) * 1000.0;
        Stats.CpuUsage = CpuUsage;

        if (json.blkio_stats != null && json.blkio_stats.io_service_bytes_recursive != null)
        {
            Stats.IORead = json.blkio_stats.io_service_bytes_recursive.First(x => x.op == "read").value;
            Stats.IOWrite = json.blkio_stats.io_service_bytes_recursive.First(x => x.op == "write").value;
        }

        if (json.networks.Any())
        {
            Stats.NetworkReceive = json.networks.First().Value.rx_bytes;
            Stats.NetworkTransmit = json.networks.First().Value.tx_bytes;
        }
        return Stats;
    }

    public long MemoryUsage { get; set; }
    public double CpuUsage { get; set; }
    public long IORead { get; set; }
    public long IOWrite { get; set; }
    public long NetworkTransmit { get; set; }
    public long NetworkReceive { get; set; }
    public DateTime Date { get; set; }
}