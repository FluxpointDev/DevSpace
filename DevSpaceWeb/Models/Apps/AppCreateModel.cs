using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class AppCreateModel
{
    [Required(ErrorMessage = "App name is required")]
    public string Name { get; set; } = null!;

    [MaxLength(300, ErrorMessage = "App description has a maximum of 300 characters")]
    public string Description { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "App vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }

    [Required(ErrorMessage = "Bot token is required")]
    public string? BotToken { get; set; }
}
