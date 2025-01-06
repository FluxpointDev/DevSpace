using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace DevSpaceWeb.Data.Users;

[CollectionName("users")]
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

    [BsonIgnore]
    public bool IsManaged => Account.ManagedAccountTeamId != null;

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!HasAvatar)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return Avatar.Url(usePng ? "png" : "webp");
    }

    public string? GetBackground()
    {
        if (BackgroundId.HasValue)
            return Background.Url("webp");

        return null;
    }

    public bool IsInstanceAdmin { get; set; }

    public UserAccount Account { get; set; } = new UserAccount();
    public UserSettings Settings { get; set; } = new UserSettings();
    public UserMfa Mfa { get; set; } = new UserMfa();
    public UserDisabled? Disabled { get; set; }

    public void UpdatePartial()
    {
        if (_DB.Users.TryGetValue(Id, out PartialUserData ud))
            ud.Update(this);
        else
            _DB.Users.Add(Id, ud);
    }
}








[CollectionName("user_roles")]
public class UserRole : MongoIdentityRole<ObjectId>
{

}
