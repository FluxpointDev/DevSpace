using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class CommandCreateModal
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name has a maximum of 100 characters")]
    [AppCommandNameValidation(ErrorMessage = "Invalid characters, requires a-z lowercase or - or _")]
    public string Name { get; set; }

    [MaxLength(300, ErrorMessage = "Description has a maximum of 300 characters")]
    public string Description { get; set; }
}
public class SlashCommandCreateModal
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name has a maximum of 100 characters")]
    [AppSlashCommandNameValidation(ErrorMessage = "Invalid characters, requires a-z lowercase.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(300, ErrorMessage = "Description has a maximum of 300 characters")]
    public string Description { get; set; }
}