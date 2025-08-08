using DevSpaceWeb.Data.Status;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Status;

public class CreateStatusMonitorModel
{
    [Required(ErrorMessage = "Monitor name is required")]
    public string? Name { get; set; }

    public StatusMonitorType Type { get; set; }

    [Required(ErrorMessage = "Source is required")]
    public string Source { get; set; }
}
