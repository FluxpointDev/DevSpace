using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Teams;

public class TeamAddUserModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [MaxLength(100, ErrorMessage = "Email has a maximum of 100 characters")]
    public string? Email { get; set; }
}
