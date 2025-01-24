using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ModifyRconModel
{
    [Required(ErrorMessage = "Server IP address is required")]
    public string Ip { get; set; }

    public short Port { get; set; }

    [Required(ErrorMessage = "Rcon password is required")]
    public string Password { get; set; }
}
