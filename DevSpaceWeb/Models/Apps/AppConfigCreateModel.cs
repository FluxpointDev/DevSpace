using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class AppConfigCreateModel
{
    [Required(ErrorMessage = "App config name is required")]
    [MaxLength(100, ErrorMessage = "App config name has a maximum of 100 characters")]
    public string Name { get; set; } = null!;

    [MaxLength(1024, ErrorMessage = "App config value has a maximum of 1024 characters")]
    public string? Value { get; set; } = null;

    public bool Sensitive { get; set; }
}
