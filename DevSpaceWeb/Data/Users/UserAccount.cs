using MongoDB.Bson;

namespace DevSpaceWeb.Data.Users;

public class UserAccount
{
    public Dictionary<string, UserSession> Sessions = new Dictionary<string, UserSession>();
    public DateTime? LoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public DateTime? EmailChangedAt { get; set; }
    public ObjectId? ManagedAccountTeamId { get; set; }
}
