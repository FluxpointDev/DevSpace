using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Status;

public class CreateStatusMonitorModel
{
    [Required(ErrorMessage = "Monitor name is required")]
    public string? Name { get; set; }
}
