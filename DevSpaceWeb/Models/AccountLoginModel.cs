using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

public class AccountLoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
