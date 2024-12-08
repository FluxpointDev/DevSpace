using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Database;
using DevSpaceWeb.Fido2;
using Fido2NetLib;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace DevSpaceWeb.Data.Users;

[CollectionName("auth_internal")]
public class AuthUser : MongoIdentityUser<ObjectId>
{
    public string? DisplayName { get; set; }
    public Guid? ResourceId { get; set; }
    public Guid? AvatarId { get; set; }
    public Guid? BackgroundId { get; set; }

    [BsonIgnore]
    public FileResource Avatar => new FileResource("Avatar", ResourceId, AvatarId);

    [BsonIgnore]
    public FileResource Background => new FileResource("Background", ResourceId, BackgroundId);

    [BsonIgnore]
    public bool HasAvatar => AvatarId != null;
    public ObjectId? ManagedAccountTeamId { get; set; }

    [BsonIgnore]
    public bool IsManaged => ManagedAccountTeamId != null;

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!HasAvatar)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return Avatar.Url(usePng ? "png" : "webp");
    }

    public bool IsInstanceAdmin;

    public UserAccount Account = new UserAccount();
    public UserMfa Mfa = new UserMfa();
    public UserDisabled? Disabled { get; set; }

    public async Task<FidoStoredCredential?> GetPasskeyByIdAsync(byte[] id)
    {
        string credentialIdString = Base64Url.Encode(id);

        FidoStoredCredential? cred = Mfa.Passkeys
            .Where(c => c.DescriptorJson != null && c.DescriptorJson.Contains(credentialIdString))
            .FirstOrDefault();

        return cred;
    }

    public Task<List<FidoStoredCredential>> GetPasskeysByUserHandleAsync(byte[] userHandle)
    {
        return Task.FromResult(Mfa.Passkeys.Where(c => c.UserHandle != null && c.UserHandle.SequenceEqual(userHandle)).ToList());
    }

    public void UpdatePartial()
    {
        if (_DB.Users.TryGetValue(Id, out PartialUserData ud))
            ud.Update(this);
        else
            _DB.Users.Add(Id, ud);
    }
}








[CollectionName("auth_roles")]
public class UserRole : MongoIdentityRole<ObjectId>
{

}
