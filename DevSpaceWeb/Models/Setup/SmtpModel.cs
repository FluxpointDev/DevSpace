using DevSpaceWeb.Data;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

public class SmtpModel
{
    [Required(ErrorMessage = "Smtp host is required")]
    [MaxLength(100, ErrorMessage = "Smtp host has a maximum of 100 characters")]
    public string? Host { get; set; }

    public int Port { get; set; } = 587;

    [Required(ErrorMessage = "Smtp email address is required")]
    [MaxLength(100, ErrorMessage = "Smtp email address has a maximum of 100 characters")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string? EmailAddress { get; set; }

    [Required(ErrorMessage = "Smtp user is required")]
    [MaxLength(100, ErrorMessage = "Smtp user has a maximum of 100 characters")]
    public string? User { get; set; }

    [Required(ErrorMessage = "Smtp password is required")]
    [MaxLength(100, ErrorMessage = "Smtp password has a maximum of 100 characters")]
    public string? Password { get; set; }

    [MaxLength(100, ErrorMessage = "Smtp email address has a maximum of 100 characters")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string? TestEmailAddress { get; set; }

    public static SmtpModel Create()
    {
        return new SmtpModel
        {
            Host = _Data.Config.Email.SmtpHost,
            Port = _Data.Config.Email.SmtpPort,
            User = _Data.Config.Email.SmtpUser,
            Password = _Data.Config.Email.SmtpPassword,
            EmailAddress = _Data.Config.Email.SenderEmailAddress
        };
    }
}
