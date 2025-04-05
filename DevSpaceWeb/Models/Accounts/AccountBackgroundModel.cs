using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public class AccountBackgroundModel
{
    [Required(ErrorMessage = "Background url is required")]
    [Url(ErrorMessage = "Background url is invalid")]
    [MaxLength(300, ErrorMessage = "Background url has a maximum of 300 characters")]
    public string? BackgroundUrl { get; set; }
}
