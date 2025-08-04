using DevSpaceWeb.Components.DynamicForm.Attributes;
using DevSpaceWeb.Models.Defaults;
using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public class AccountChangePasswordWithCurrentModel : CurrentPasswordModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(100, ErrorMessage = "Email has a maximum of 100 characters")]
    [Placeholder("name@email.com")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password requires 8 characters")]
    [MaxLength(100, ErrorMessage = "Password has a maxiumum 100 characters")]
    [PasswordValidation(ErrorMessage = "Password is not secure enough")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string? RepeatPassword { get; set; }
}
