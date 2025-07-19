using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Projects;

public class CreateProjectModel
{
    [Required(ErrorMessage = "Project name is required")]
    public string? Name { get; set; }

    [MaxLength(100, ErrorMessage = "Project vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}