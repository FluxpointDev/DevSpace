using DevSpaceWeb.Components.DynamicForm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Account;

public class AccountBackgroundModel
{
    [Required(ErrorMessage = "Background url is required")]
    [ImageUrl(ErrorMessage = "Background url is invalid")]
    [MaxLength(300, ErrorMessage = "Background url has a maximum of 300 characters")]
    [Placeholder("https://domain.com/image.png")]
    public string BackgroundUrl { get; set; } = null!;
}
