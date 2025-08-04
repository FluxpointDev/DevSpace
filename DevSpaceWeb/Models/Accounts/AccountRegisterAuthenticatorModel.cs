using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public class AccountRegisterAuthenticatorModel
{
    [Required(ErrorMessage = "Authenticator code is required")]
    [Placeholder("000000")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Device name is required")]
    [MaxLength(100, ErrorMessage = "Device name has a maximum of 100 characters")]
    public string? DeviceName { get; set; }
}
