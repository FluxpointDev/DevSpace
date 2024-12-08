using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Defaults;

public class ReasonModel
{
    [MaxLength(300, ErrorMessage = "Reason has a maximum of 300 characters")]
    public string Reason { get; set; }
}
