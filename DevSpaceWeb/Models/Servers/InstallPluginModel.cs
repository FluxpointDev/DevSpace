using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Servers;

public class InstallPluginModel
{
    [Required]
    public string? Name { get; set; }
}
