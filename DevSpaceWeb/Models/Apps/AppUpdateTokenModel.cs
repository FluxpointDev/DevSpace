using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class AppUpdateTokenModel
{
    [Required(ErrorMessage = "Bot token is required")]
    [DataType(DataType.Password)]
    public string? BotToken { get; set; }
}
