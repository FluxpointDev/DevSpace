using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Defaults;

public class EmailModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(100, ErrorMessage = "Email has a maximum of 100 characters")]
    [Placeholder("name@email.com")]
    public string? Email { get; set; }
}
