using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Fido2NetLib;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace DevSpaceWeb.Data;

[CollectionName("auth_internal")]
public class AuthUser : MongoIdentityUser<ObjectId>
{
    public string? DisplayName { get; set; }
    public bool HasAvatar() => AvatarId.HasValue;
    public Guid? ResourceId { get; set; }
    public Guid? AvatarId { get; set; }
    public ObjectId? ManagedAccountTeamId { get; set; }
    public bool IsManaged()
    {
        return ManagedAccountTeamId != null;
    }

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!HasAvatar())
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return _Data.Config.Instance.GetPublicUrl() + "/public/resources/" + ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";
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

    public void UpdatePartial()
    {
        if (_DB.Users.TryGetValue(Id, out PartialUserData ud))
            ud.Update(this);
        else
            _DB.Users.Add(Id, ud);
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
