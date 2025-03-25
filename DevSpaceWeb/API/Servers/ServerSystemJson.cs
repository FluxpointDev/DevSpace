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

    public string id;
    public string name;
    public string agent_version;
    public string agent_net;
    public string uptime_text;
    public long uptime_number;
    public string operating_system;
    public long cpu_count;
    public long storage_total_size;
    public long storage_free_size;
    public string storage_text;
    public long ram_total;
    public string ram_text;
    public ServerDockerJson docker;
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
    public string ip;
    public string kernel_version;
    public string root_directory;
    public string storage_driver;
    public string logging_driver;
    public string system_driver;
    public IList<string> volume_plugins;
    public IList<string> network_plugins;
    public IList<string> log_plugins;
    public IList<string> auth_plugins;
    public string hostname;
    public IList<string> warnings;
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

    public int networks_count;
    public string docker_version;
    public string api_version;
    public int stacks_count;
    public long all_containers_count;
    public long running_containers_count;
    public long stopped_containers_count;
    public long images_count;
    public int volumes_count;
    public int plugins_count;
}