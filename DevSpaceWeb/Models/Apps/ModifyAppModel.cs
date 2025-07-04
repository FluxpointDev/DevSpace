using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class ModifyAppModel
{
    [Required(ErrorMessage = "App name is required")]
    public string Name { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "App vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
