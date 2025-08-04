using DevSpaceWeb.Components.DynamicForm.Attributes;
using DevSpaceWeb.Data.Consoles;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class SetupConsoleModel
{
    [Required(ErrorMessage = "Server name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Server IP address is required")]
    [Placeholder("1.2.3.4")]
    public string? Ip { get; set; }

    public short Port { get; set; }

    [Required(ErrorMessage = "Rcon password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Rcon type is required")]
    public ConsoleType Type { get; set; }

    [MaxLength(100, ErrorMessage = "Console vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
