using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Accounts;

public class AccountItemEditModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(50, ErrorMessage = "Name has a max of 50 characters.")]
    public string Name { get; set; }
}
