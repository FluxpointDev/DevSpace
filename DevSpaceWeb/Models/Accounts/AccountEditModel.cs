using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Accounts;

public class AccountEditModel
{
    [MinLength(3, ErrorMessage = "Username requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Username has a maximum of 32 characters")]
    [UsernameValidation(ErrorMessage = "Username invalid, use characters from A-Z, 0-9 and - . _")]
    public string? Username { get; set; }

    [MaxLength(32, ErrorMessage = "Display name has a maximum of 32 characters")]
    public string? DisplayName { get; set; }
}
