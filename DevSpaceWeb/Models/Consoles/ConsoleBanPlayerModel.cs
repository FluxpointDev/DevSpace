using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ConsoleBanPlayerModel : ConsoleReasonModel
{
    [Required(ErrorMessage = "Player is required")]
    [Display(Name = "Player or IP")]
    public string Player { get; set; }
}
