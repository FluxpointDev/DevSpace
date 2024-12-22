using DevSpaceWeb.Fido2;
using Fido2NetLib;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace DevSpaceWeb.Data.Users;

public class UserMfa
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? EmailCodeLastSentAt { get; set; }
    public DateTime? EmailCodeLastUsedAt { get; set; }
    public DateTime? AuthenticatorLastRegisteredAt { get; set; }
    public DateTime? AuthenticatorLastUsedAt { get; set; }
    public Dictionary<string, bool> AuthenticatorDevices { get; set; } = new Dictionary<string, bool>();

    public bool HasAny2FA()
    {
        if (AuthenticatorDevices.Any() || Passkeys.Any())
            return true;
        return false;
    }
    public void Disable2FA()
    {
        IsTwoFactorEnabled = false;
        AuthenticatorLastRegisteredAt = null;
        AuthenticatorLastUsedAt = null;
        AuthenticatorDevices.Clear();
        PasskeyId = null;
        PasskeyLastRegisteredAt = null;
        PasskeyLastUsedAt = null;
        PasskeyLastUsedDevice = null;
        Passkeys.Clear();
        RecoveryCodeCreatedAt = null;
        RecoveryCodeLastUsedAt = null;
        RecoveryCode = null;
    }

    public ObjectId? PasskeyId { get; set; }
    public DateTime? PasskeyLastRegisteredAt { get; set; }
    public DateTime? PasskeyLastUsedAt { get; set; }
    public string? PasskeyLastUsedDevice { get; set; }
    public List<FidoStoredCredential> Passkeys { get; set; } = new List<FidoStoredCredential>();

    public DateTime? RecoveryCodeCreatedAt { get; set; }
    public DateTime? RecoveryCodeLastUsedAt { get; set; }
    public string? RecoveryCode { get; set; }

    public async Task<FidoStoredCredential?> GetPasskeyByIdAsync(byte[] id)
    {
        string credentialIdString = Base64Url.Encode(id);

        FidoStoredCredential? cred = Passkeys
            .Where(c => c.DescriptorJson != null && c.DescriptorJson.Contains(credentialIdString))
            .FirstOrDefault();

        return cred;
    }

    public Task<List<FidoStoredCredential>> GetPasskeysByUserHandleAsync(byte[] userHandle)
    {
        return Task.FromResult(Passkeys.Where(c => c.UserHandle != null && c.UserHandle.SequenceEqual(userHandle)).ToList());
    }
}
