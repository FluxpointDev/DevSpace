using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class AppUpdateTokenModel
{
    [Required(ErrorMessage = "Bot token is required")]
    public string? BotToken { get; set; }
}
