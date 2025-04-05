using DevSpaceWeb.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Docker;

public class ContainerRenameModel
{
    [Required(ErrorMessage = "Container name is required")]
    [ContainerNameValidation(ErrorMessage = "Container name is invalid: need to be all lowercase, 2-255 characters and optionally include special characters such as . _ -")]
    public string? Name { get; set; }
}
