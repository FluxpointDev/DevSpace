using DevSpaceWeb.Models.Defaults;
using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Accounts;

public class AccountEditModel : UsernameModel
{
    [MaxLength(32, ErrorMessage = "Display name has a maximum of 32 characters")]
    public string DisplayName { get; set; }
}
