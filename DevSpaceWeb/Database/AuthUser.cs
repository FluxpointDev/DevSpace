using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Fido2;
using Fido2NetLib;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace DevSpaceWeb.Database;

[CollectionName("auth_internal")]
public class AuthUser : MongoIdentityUser<ObjectId>
{
    public bool HasAvatar;
    public int AvatarVersion;

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!HasAvatar)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return "";
    }

    public bool IsInstanceAdmin;

    public AuthUserMfa Auth = new AuthUserMfa();

    public bool HasPasskeys()
    {
        return Auth.Passkeys.Any();
    }

    public async Task<FidoStoredCredential?> GetPasskeyByIdAsync(byte[] id)
    {
        var credentialIdString = Base64Url.Encode(id);

        var cred = Auth.Passkeys
            .Where(c => c.DescriptorJson != null && c.DescriptorJson.Contains(credentialIdString))
            .FirstOrDefault();

        return cred;
    }

    public Task<List<FidoStoredCredential>> GetPasskeysByUserHandleAsync(byte[] userHandle)
    {
        return Task.FromResult(Auth.Passkeys.Where(c => c.UserHandle != null && c.UserHandle.SequenceEqual(userHandle)).ToList());
    }
}

public class AuthUserSession
{

}

public class AuthUserMfa
{
    public Dictionary<string, AuthUserSession> Sessions = new Dictionary<string, AuthUserSession>();
    public bool IsTwoFactorEnabled { get; set; } = false;
    public DateTimeOffset? LoginAt { get; set; }
    public DateTimeOffset? PasswordChangedAt { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
    public DateTimeOffset? EmailChangedAt { get; set; }

    public DateTimeOffset? EmailCodeLastSentAt { get; set; }
    public DateTimeOffset? EmailCodeLastUsedAt { get; set; }

    public DateTimeOffset? AuthenticatorLastRegisteredAt { get; set; }
    public DateTimeOffset? AuthenticatorLastUsedAt { get; set; }
    public Dictionary<string, bool> AuthenticatorDevices { get; set; } = new Dictionary<string, bool>();

    public ObjectId? PasskeyId { get; set; }
    public DateTimeOffset? PasskeyLastRegisteredAt { get; set; }
    public DateTimeOffset? PasskeyLastUsedAt { get; set; }
    public string? PasskeyLastUsedDevice { get; set; }
    public List<FidoStoredCredential> Passkeys { get; set; } = new List<FidoStoredCredential>();

    public DateTimeOffset? RecoveryCodeCreatedAt { get; set; }
    public DateTimeOffset? RecoveryCodeLastUsedAt { get; set; }
    public string? RecoveryCode { get; set; }
}

[CollectionName("auth_roles")]
public class ApplicationRole : MongoIdentityRole<ObjectId>
{

}
