using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class CommandCreateModal
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name has a maximum of 100 characters")]
    public string Name { get; set; }

    [MaxLength(300, ErrorMessage = "Description has a maximum of 300 characters")]
    public string Description { get; set; }
}
