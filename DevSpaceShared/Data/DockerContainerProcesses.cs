using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerContainerProcesses
{
    public string? ContainerName { get; set; }
    public ContainerProcessesResponse? Data { get; set; }

    public bool IsNotRunning { get; set; }
}
public class DockerContainerStat
{
    public long MemoryUsage { get; set; }
    public float CpuUsage { get; set; }
    public long IORead { get; set; }
    public long IOWrite { get; set; }
    public long NetworkTransmit { get; set; }
    public long NetworkReceive { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}