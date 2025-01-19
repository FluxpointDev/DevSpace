using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ConsoleReasonModel
{
    [MaxLength(300, ErrorMessage = "Reason has a maxiumum 300 characters")]
    public string Reason { get; set; }
}
