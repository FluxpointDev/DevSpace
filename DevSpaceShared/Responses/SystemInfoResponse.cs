using Newtonsoft.Json;

namespace DevSpaceShared.Responses;
public class SystemInfoResponse
{
    public string? AgentVersion { get; set; }
    public string? AgentNET { get; set; }
    public long Uptime { get; set; }
    public string? OperatingSystem { get; set; }
    public long CpuCount { get; set; }
    public long StorageTotalSize { get; set; }
    public long StorageFreeSize { get; set; }
    public long RamTotal { get; set; }

    public required SystemDockerInfo Docker { get; set; }

    [JsonIgnore]
    public bool IsWindows => OperatingSystem == "Docker Desktop";

    public string GetUptimeText()
    {
        TimeSpan runtime = TimeSpan.FromMilliseconds(Uptime);
        if (runtime.Days != 0)
            return $"{runtime.Days} Days {runtime.Hours} Hours";
        else if (runtime.Hours != 0)
            return $"{runtime.Hours} Hours {runtime.Minutes} Minutes";
        else if (runtime.Minutes != 0)
            return $"{runtime.Minutes} Minutes {runtime.Seconds} Seconds";
        return $"{runtime.Seconds} Seconds";
    }
}
public class SystemInfoFullResponse : SystemInfoResponse
{
    public string? KernelVersion { get; set; }
    public string? RootDirectory { get; set; }
    public string? StorageDriver { get; set; }
    public string? LoggingDriver { get; set; }
    public string? SystemDriver { get; set; }
    public IList<string>? VolumePlugins { get; set; }
    public IList<string>? NetworkPlugins { get; set; }
    public IList<string>? LogPlugins { get; set; }
    public IList<string>? AuthPlugins { get; set; }
    public string? Hostname { get; set; }
    public IList<string>? Warnings { get; set; }
    public SystemInfoDriveResponse[]? Drives { get; set; }
}
public class SystemInfoDriveResponse
{
    public string? Name { get; set; }
    public string? Directory { get; set; }
    public long TotalSize { get; set; }
    public long FreeSize { get; set; }
}
public class SystemDockerInfo
{
    public int Networks { get; set; }
    public string? DockerVersion { get; set; }
    public string? APIVersion { get; set; }
    public int Stacks { get; set; }
    public long AllContainers { get; set; }
    public long RunningContainers { get; set; }
    public long StoppedContainers { get; set; }
    public long Images { get; set; }
    public int Volumes { get; set; }
    public int Plugins { get; set; }
}