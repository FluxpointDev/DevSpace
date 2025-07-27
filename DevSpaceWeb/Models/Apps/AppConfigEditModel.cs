using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

public class AppConfigEditModel
{
    [MaxLength(1024, ErrorMessage = "App config value has a maximum of 1024 characters")]
    public string? Value { get; set; } = null;
}
