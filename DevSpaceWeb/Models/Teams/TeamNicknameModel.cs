using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Teams;

public class TeamNicknameModel
{
    [MaxLength(32, ErrorMessage = "Nickname has a maximum of 32 characters")]
    public string? Nickname { get; set; }
}
