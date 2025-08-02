using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Status;

public class CreateStatusPageModel
{
    [Required(ErrorMessage = "Page name is required")]
    public string? Name { get; set; }
}
