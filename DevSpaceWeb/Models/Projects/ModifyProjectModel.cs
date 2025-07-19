using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ModifyProjectModel
{
    [Required(ErrorMessage = "Project name is required")]
    public string? Name { get; set; }

    [MaxLength(100, ErrorMessage = "Project vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
