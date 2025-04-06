namespace DevSpaceShared.Data;
public class DockerStatJson
{
    public DockerStatMemoryJson memory_stats;
    public Dictionary<string, DockerStatNetworkJson> networks = new Dictionary<string, DockerStatNetworkJson>();
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