using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ModifyConsoleModel
{
    [Required(ErrorMessage = "Console name is required")]
    public string Name { get; set; }

    [MaxLength(100, ErrorMessage = "Console vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
