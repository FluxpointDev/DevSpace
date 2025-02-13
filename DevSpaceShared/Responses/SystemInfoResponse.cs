namespace DevSpaceShared.Responses;
public class SystemInfoResponse
{
    public string AgentVersion;
    public string AgentNET;
    public string Uptime;
    public string OperatingSystem;
    public long CpuCount;
    public long RamTotal;
    public string KernelVersion;
    public SystemDockerInfo Docker;
}
public class SystemDockerInfo
{
    public int Networks;
    public string DockerVersion;
    public int Stacks;
    public long AllContainers;
    public long RunningContainers;
    public long StoppedContainers;
    public long PausedContainers;
    public long Images;
    public int Volumes;
    public int Plugins;
}