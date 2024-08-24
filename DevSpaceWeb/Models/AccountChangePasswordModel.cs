using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

public class AccountChangePasswordModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password requires 8 characters")]
    [MaxLength(100, ErrorMessage = "Password has a maxiumum 100 characters")]
    [PasswordValidation(ErrorMessage = "Password is not secure enough")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string RepeatPassword { get; set; }
}
