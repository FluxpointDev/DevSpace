using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Setup;

public class ServerSetupEdgeModel
{
    [Required(ErrorMessage = "Server name is required")]
    public string Name { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "Server vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
public class ServerSetupClientModel
{
    [Required(ErrorMessage = "Server name is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Server IP address is required")]
    [Placeholder("1.2.3.4")]
    public string Ip { get; set; } = null!;

    public short Port { get; set; } = 5555;

    [Required(ErrorMessage = "Agent key is required")]
    [DataType(DataType.Password)]
    public string AgentKey { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "Server vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
