namespace DevSpaceShared.Data;
public class DockerStatJson
{
    public DockerStatMemoryJson memory_stats;
    public DockerStatCpuJson cpu_stats;
    public DockerStatCpuJson precpu_stats;
    public DockerStatBlkioJson? blkio_stats;
    //public DockerStatCpuJson GetCpuStats()
    //{
    //    if (cpu_stats == null)
    //        return precpu_stats;

    //    return cpu_stats;
    //}
    public Dictionary<string, DockerStatNetworkJson>? networks;
}
public class DockerStatCpuJson
{
    public long online_cpus;
    public long system_cpu_usage;
    public DockerStatCpuUsageJson cpu_usage;
}
public class DockerStatBlkioJson
{
    public DockerStatBlkioServiceJson[]? io_service_bytes_recursive;
}
public class DockerStatBlkioServiceJson
{
    public long value;
    public string op;
}
public class DockerStatCpuUsageJson
{
    public long total_usage;
    public long[] percpu_usage;
}
public class DockerStatMemoryJson
{
    public long usage;
    public DockerStatMemoryStatsJson stats;
}
public class DockerStatMemoryStatsJson
{
    public long cache;
}
public class DockerStatNetworkJson
{
    public long rx_bytes;
    public long tx_bytes;
}