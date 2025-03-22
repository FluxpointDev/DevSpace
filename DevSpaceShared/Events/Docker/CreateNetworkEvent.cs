namespace DevSpaceShared.Events.Docker;

public class CreateNetworkEvent
{
    public string Name { get; set; }

    public string DriverType { get; set; }

    public Dictionary<string, string> DriverOptions { get; set; } = new Dictionary<string, string>();

    public string IPv4Subnet { get; set; }
    public string IPv4Gateway { get; set; }
    public string IPv4Range { get; set; }

    public string IPv6Subnet { get; set; }
    public string IPv6Gateway { get; set; }
    public string IPv6Range { get; set; }

    public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();

    public bool IsIsolated { get; set; }
    public bool IsManualAttach { get; set; }
}
