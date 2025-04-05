using DevSpaceWeb.Data;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

public class InstanceBasicModel
{
    [Required(ErrorMessage = "Instance name is required")]
    [MinLength(3, ErrorMessage = "Instance name requires minimum of 3 characters")]
    [MaxLength(32, ErrorMessage = "Instance name has a maximum of 32 characters")]
    public string? Name { get; set; }

    [MaxLength(300, ErrorMessage = "Instance description has a maximum of 300 characters")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(300, ErrorMessage = "Public domain has a maximum of 300 characters")]
    public string? PublicDomain { get; set; }

    public static InstanceBasicModel Create()
    {
        return new InstanceBasicModel
        {
            Name = _Data.Config.Instance.Name,
            Description = _Data.Config.Instance.Description,
            PublicDomain = _Data.Config.Instance.PublicDomain
        };
    }
}
