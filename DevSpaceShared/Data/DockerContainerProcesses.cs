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
            Date = DateTime.UtcNow
        };

        // cgroups v1
        if (json.memory_stats.stats.cache != 0)
            Stats.MemoryUsage = json.memory_stats.stats.cache;
        else
            Stats.MemoryUsage = json.memory_stats.usage - json.memory_stats.stats.cache;

        long NumberCpus = 0;

        // cgroups v1
        if (json.cpu_stats.cpu_usage.percpu_usage != null)
            NumberCpus = json.cpu_stats.cpu_usage.percpu_usage.Length;
        else
            NumberCpus = json.cpu_stats.online_cpus;

        double CpuDelta = json.cpu_stats.cpu_usage.total_usage - json.precpu_stats.cpu_usage.total_usage;
        double SystemCpuDelta = json.cpu_stats.system_cpu_usage - json.precpu_stats.system_cpu_usage;
        double CpuUsage = ((CpuDelta / SystemCpuDelta) * NumberCpus) * 100.0;
        Stats.CpuUsage = Double.Round(CpuUsage, 1, MidpointRounding.ToEven);

        if (json.blkio_stats != null && json.blkio_stats.io_service_bytes_recursive != null)
        {
            try
            {
                Stats.IORead = json.blkio_stats.io_service_bytes_recursive.First(x => x.op == "Read" || x.op == "read").value;
                Stats.IOWrite = json.blkio_stats.io_service_bytes_recursive.First(x => x.op == "Write" || x.op == "write").value;
            }
            catch { }
        }

        if (json.networks != null)
        {
            Stats.Networks = new Dictionary<string, DockerContainerStatsNetwork>();
            foreach (var i in json.networks)
            {
                Stats.Networks.Add(i.Key, new DockerContainerStatsNetwork
                {
                    NetworkReceive = i.Value.rx_bytes,
                    NetworkTransmit = i.Value.tx_bytes
                });
            }
        }

        return Stats;
    }

    public double MemoryUsage { get; set; }
    public double CpuUsage { get; set; }
    public double IORead { get; set; }
    public double IOWrite { get; set; }

    public Dictionary<string, DockerContainerStatsNetwork>? Networks;

    public double NetworkTransmit => string.IsNullOrEmpty(SelectedNetwork) ? 0 : Networks?.GetValueOrDefault(SelectedNetwork)?.NetworkTransmit ?? 0;
    public double NetworkReceive => string.IsNullOrEmpty(SelectedNetwork) ? 0 : Networks?.GetValueOrDefault(SelectedNetwork)?.NetworkReceive ?? 0;

    public string? SelectedNetwork { get; set; }
    public DateTime Date { get; set; }
}
public class DockerContainerStatsNetwork
{
    public double NetworkTransmit { get; set; }
    public double NetworkReceive { get; set; }
}