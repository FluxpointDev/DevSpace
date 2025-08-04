using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Servers;

public class ModifyServerConnectionModel
{
    [Required(ErrorMessage = "Server IP address is required")]
    [Placeholder("1.2.3.4")]
    public string Ip { get; set; } = null!;

    public short Port { get; set; }

    [Required(ErrorMessage = "Agent key is required")]
    [DataType(DataType.Password)]
    public string AgentKey { get; set; } = null!;
}
