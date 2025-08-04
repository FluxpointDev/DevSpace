using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Servers;

public class InstallPluginModel
{
    [Required(ErrorMessage = "Plugin name is required")]
    public string? Name { get; set; }
}
