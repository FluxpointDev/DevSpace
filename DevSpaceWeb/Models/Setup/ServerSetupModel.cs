using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Setup;

public class ServerSetupModel
{
    [Required(ErrorMessage = "Server name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Server IP address is required")]
    public string Ip { get; set; }

    [Required(ErrorMessage = "Agent key is required")]
    public string AgentKey { get; set; }
}
