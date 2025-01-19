using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Teams;

public class TeamBasicModel
{
    [Required(ErrorMessage = "Team name is required")]
    [MinLength(3, ErrorMessage = "Team name requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Team name has a maximum of 32 characters")]
    public string Name { get; set; }

    [MaxLength(100, ErrorMessage = "Team vanity url has a maximum of 100 characters")]
    public string? VanityUrl { get; set; }
}
