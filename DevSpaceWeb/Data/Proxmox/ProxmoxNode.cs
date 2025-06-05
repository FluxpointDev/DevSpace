namespace DevSpaceWeb.Data.Proxmox;

public class ProxmoxNode
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public long MaxCPU { get; set; }
    public long MaxStorage { get; set; }
    public long MaxMemory { get; set; }
    public long UsedMemory { get; set; }
    public long Uptime { get; set; }
}
