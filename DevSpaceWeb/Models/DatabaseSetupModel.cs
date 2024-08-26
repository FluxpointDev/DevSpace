using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

public class DatabaseSetupModel
{
    [Required]
    [MaxLength(100, ErrorMessage = "Database name has a maximum of 100 characters")]
    public string Name { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Database IP has a maximum of 100 characters")]
    public string IP { get; set; }

    public int Port { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Database username has a maximum of 100 characters")]
    public string Username { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Database password has a maximum of 100 characters")]
    public string Password { get; set; }
}
