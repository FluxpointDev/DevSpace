using DevSpaceWeb.Models.Defaults;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public class AccountLoginModel : CurrentPasswordModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(100, ErrorMessage = "Email has a maximum of 100 characters")]
    public string Email { get; set; }
}
