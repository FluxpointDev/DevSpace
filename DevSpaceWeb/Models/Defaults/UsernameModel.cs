using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Defaults;

public class UsernameModel
{
    public static string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Username has a maximum of 32 characters")]
    [UsernameValidation(ErrorMessage = "Username invalid, use characters from A-Z, 0-9 and - . _")]
    public string Username { get; set; }
}
