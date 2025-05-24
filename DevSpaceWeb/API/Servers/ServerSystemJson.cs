using DevSpaceShared.Responses;
using DevSpaceWeb.Data.Servers;

namespace DevSpaceWeb.API.Servers;

public class ServerSystemJson
{
    public ServerSystemJson(ServerData data, SystemInfoResponse system)
    {
        id = data.Id.ToString();
        name = data.Name;
        agent_version = system.AgentVersion;
        agent_net = system.AgentNET;
        uptime_text = system.GetUptimeText();
        uptime_number = system.Uptime;
        operating_system = system.OperatingSystem;
        cpu_count = system.CpuCount;
        storage_total_size = system.StorageTotalSize;
        storage_free_size = system.StorageFreeSize;
        storage_text = $"{Utils.SizeSuffix(system.StorageTotalSize - system.StorageFreeSize, 0)} / {Utils.SizeSuffix(system.StorageTotalSize, 0)} ({100 * (system.StorageTotalSize - system.StorageFreeSize) / system.StorageTotalSize}%)";
        ram_total = system.RamTotal;
        ram_text = Utils.ToPrettySize(system.RamTotal);
        docker = new ServerDockerJson(system.Docker);
    }

    public string id { get; set; }
    public string name { get; set; }
    public string? agent_version { get; set; }
    public string? agent_net { get; set; }
    public string uptime_text { get; set; }
    public long uptime_number { get; set; }
    public string? operating_system { get; set; }
    public long cpu_count { get; set; }
    public long storage_total_size { get; set; }
    public long storage_free_size { get; set; }
    public string storage_text { get; set; }
    public long ram_total { get; set; }
    public string ram_text { get; set; }
    public ServerDockerJson docker { get; set; }
}
public class ServerHostJson : ServerSystemJson
{
    public ServerHostJson(ServerData server, SystemInfoFullResponse host) : base(server, new SystemInfoResponse
    {
        AgentNET = host.AgentNET,
        AgentVersion = host.AgentVersion,
        CpuCount = host.CpuCount,
        OperatingSystem = host.OperatingSystem,
        RamTotal = host.RamTotal,
        StorageFreeSize = host.StorageFreeSize,
        StorageTotalSize = host.StorageTotalSize,
        Uptime = host.Uptime,
        Docker = host.Docker
    })
    {
        ip = server.AgentIp;
        kernel_version = host.KernelVersion;
        root_directory = host.RootDirectory;
        storage_driver = host.StorageDriver;
        logging_driver = host.LoggingDriver;
        system_driver = host.SystemDriver;
        volume_plugins = host.VolumePlugins;
        network_plugins = host.NetworkPlugins;
        log_plugins = host.LogPlugins;
        auth_plugins = host.AuthPlugins;
        hostname = host.Hostname;
        warnings = host.Warnings;
    }
    public string ip { get; set; }
    public string? kernel_version { get; set; }
    public string? root_directory { get; set; }
    public string? storage_driver { get; set; }
    public string? logging_driver { get; set; }
    public string? system_driver { get; set; }
    public IList<string>? volume_plugins { get; set; }
    public IList<string>? network_plugins { get; set; }
    public IList<string>? log_plugins { get; set; }
    public IList<string>? auth_plugins { get; set; }
    public string? hostname { get; set; }
    public IList<string>? warnings { get; set; }
}
public class ServerDockerJson
{
    public ServerDockerJson(SystemDockerInfo docker)
    {
        networks_count = docker.Networks;
        docker_version = docker.DockerVersion;
        api_version = docker.APIVersion;
        stacks_count = docker.Stacks;
        all_containers_count = docker.AllContainers;
        running_containers_count = docker.RunningContainers;
        stopped_containers_count = docker.StoppedContainers;
        images_count = docker.Images;
        volumes_count = docker.Volumes;
        plugins_count = docker.Plugins;
    }

    public int networks_count { get; set; }
    public string? docker_version { get; set; }
    public string? api_version { get; set; }
    public int stacks_count { get; set; }
    public long all_containers_count { get; set; }
    public long running_containers_count { get; set; }
    public long stopped_containers_count { get; set; }
    public long images_count { get; set; }
    public int volumes_count { get; set; }
    public int plugins_count { get; set; }
}