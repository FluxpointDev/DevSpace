using DevSpaceShared;
using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class ContainerStatsJson
{
    public ContainerStatsJson(DockerContainerStats stats)
    {
        memory_usage = new ContainerStatsInfoJson
        {
            raw = Convert.ToInt64(stats.MemoryUsage),
            nearest = Convert.ToInt64(stats.MemoryUsage.RoundNearestBytes()),
        };
        memory_usage.text = Utils.SizeSuffix(memory_usage.nearest);

        cpu_usage = stats.CpuUsage;

        io_read_total = new ContainerStatsInfoJson
        {
            raw = Convert.ToInt64(stats.IORead),
            nearest = Convert.ToInt64(stats.IORead.RoundNearestBytes())
        };
        io_read_total.text = Utils.SizeSuffix(io_read_total.nearest);

        io_write_total = new ContainerStatsInfoJson
        {
            raw = Convert.ToInt64(stats.IOWrite),
            nearest = Convert.ToInt64(stats.IOWrite.RoundNearestBytes())
        };
        io_write_total.text = Utils.SizeSuffix(io_write_total.nearest);

        if (stats.Networks != null)
            networks = stats.Networks.ToDictionary(x => x.Key, x => new ContainerStatsNetworkJson(x.Value));
        else
            networks = [];

    }

    public ContainerStatsInfoJson memory_usage { get; set; }
    public ContainerStatsInfoJson io_read_total { get; set; }
    public ContainerStatsInfoJson io_write_total { get; set; }
    public double cpu_usage { get; set; }
    public Dictionary<string, ContainerStatsNetworkJson> networks { get; set; }
}
public class ContainerStatsInfoJson
{
    public string? text { get; set; }
    public long raw { get; set; }
    public long nearest { get; set; }
}
public class ContainerStatsNetworkJson
{
    public ContainerStatsNetworkJson(DockerContainerStatsNetwork network)
    {
        network_transmit_total = new ContainerStatsInfoJson
        {
            raw = Convert.ToInt64(network.NetworkTransmit),
            nearest = Convert.ToInt64(network.NetworkTransmit.RoundNearestBytes())
        };
        network_transmit_total.text = Utils.SizeSuffix(network_transmit_total.nearest);

        network_receive_total = new ContainerStatsInfoJson
        {
            raw = Convert.ToInt64(network.NetworkReceive),
            nearest = Convert.ToInt64(network.NetworkReceive.RoundNearestBytes())
        };
        network_receive_total.text = Utils.SizeSuffix(network_receive_total.nearest);
    }

    public ContainerStatsInfoJson network_transmit_total { get; set; }
    public ContainerStatsInfoJson network_receive_total { get; set; }
}
