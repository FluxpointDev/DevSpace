using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Defaults;

public class CurrentPasswordModel
{
    [Required(ErrorMessage = "Password is required")]
    [MaxLength(100, ErrorMessage = "Password has a maxiumum 100 characters")]
    public string CurrentPassword { get; set; }
}
