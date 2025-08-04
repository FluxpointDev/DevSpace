using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ModifyRconModel
{
    [Required(ErrorMessage = "Server IP address is required")]
    [Placeholder("1.2.3.4")]
    public string Ip { get; set; } = null!;

    public short Port { get; set; }

    [Required(ErrorMessage = "Rcon password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
