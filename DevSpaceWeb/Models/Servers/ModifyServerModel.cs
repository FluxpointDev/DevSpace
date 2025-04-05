using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ModifyServerModel
{
    [Required(ErrorMessage = "Server name is required")]
    public string? Name { get; set; }

    [MaxLength(100, ErrorMessage = "Server vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
