using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Teams;

public class TeamRoleModel
{
    [Required(ErrorMessage = "Role name is required")]
    [MinLength(3, ErrorMessage = "Role name requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Role name has a maximum of 32 characters")]
    public string? Name { get; set; }

    [MaxLength(300, ErrorMessage = "Role description has a maximum of 300 characters")]
    public string? Description { get; set; }
}
