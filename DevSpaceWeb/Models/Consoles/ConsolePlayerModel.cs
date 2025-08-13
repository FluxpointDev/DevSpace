using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ConsolePlayerModel
{
    [Required(ErrorMessage = "Player is required")]
    [Display(Name = "Player")]
    public string Player { get; set; }
}
