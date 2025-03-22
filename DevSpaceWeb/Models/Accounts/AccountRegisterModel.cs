using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public sealed class AccountRegisterModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(100, ErrorMessage = "Email has a maximum of 100 characters")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Username has a maximum of 32 characters")]
    [UsernameValidation(ErrorMessage = "Username invalid, use characters from A-Z, 0-9 and - . _")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password requires 8 characters")]
    [MaxLength(100, ErrorMessage = "Password has a maxiumum 100 characters")]
    [PasswordValidation(ErrorMessage = "Password is not secure enough")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string RepeatPassword { get; set; }
}
