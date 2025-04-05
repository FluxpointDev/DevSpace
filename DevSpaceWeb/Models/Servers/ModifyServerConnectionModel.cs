using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Servers;

public class ModifyServerConnectionModel
{
    [Required(ErrorMessage = "Server IP address is required")]
    public string? Ip { get; set; }

    public short Port { get; set; }

    [Required(ErrorMessage = "Agent key is required")]
    public string? AgentKey { get; set; }
}
