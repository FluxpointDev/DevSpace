using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Consoles;

public class ConsoleMessageModel
{
    [MaxLength(1000, ErrorMessage = "Message has a maxiumum 1000 characters")]
    public string? Message { get; set; }
}
