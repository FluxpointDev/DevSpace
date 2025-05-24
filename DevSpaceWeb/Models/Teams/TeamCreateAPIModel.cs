using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Teams;

public class TeamCreateAPIModel
{
    [Required(ErrorMessage = "API client name is required")]
    [MinLength(3, ErrorMessage = "API client name requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "API client name has a maximum of 32 characters")]
    public string Name { get; set; } = null!;
}
