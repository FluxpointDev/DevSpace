using DevSpaceWeb.Fido2;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace DevSpaceWeb.Data.Users;

public class UserMfa
{
    public bool IsTwoFactorEnabled { get; set; } = false;
    public DateTime? EmailCodeLastSentAt { get; set; }
    public DateTime? EmailCodeLastUsedAt { get; set; }

    public DateTime? AuthenticatorLastRegisteredAt { get; set; }
    public DateTime? AuthenticatorLastUsedAt { get; set; }
    public Dictionary<string, bool> AuthenticatorDevices { get; set; } = new Dictionary<string, bool>();

    public ObjectId? PasskeyId { get; set; }
    public DateTime? PasskeyLastRegisteredAt { get; set; }
    public DateTime? PasskeyLastUsedAt { get; set; }
    public string? PasskeyLastUsedDevice { get; set; }
    public List<FidoStoredCredential> Passkeys { get; set; } = new List<FidoStoredCredential>();

    [BsonIgnore]
    [JsonIgnore]
    public bool HasPasskeys => Passkeys.Any();

    public DateTime? RecoveryCodeCreatedAt { get; set; }
    public DateTime? RecoveryCodeLastUsedAt { get; set; }
    public string? RecoveryCode { get; set; }
}
