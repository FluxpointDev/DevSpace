using AspNetCore.Identity.MongoDbCore.Models;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using Radzen;

namespace DevSpaceWeb.Data.Users;

[CollectionName("users")]
public class AuthUser : MongoIdentityUser<ObjectId>
{
    public string? DisplayName { get; set; }

    public Guid? ResourceId { get; set; }

    public Guid? AvatarId { get; set; }

    [Obsolete]
    public Guid? BackgroundId { get; set; }

    [BsonIgnore]
    public FileResource Avatar => new FileResource("Avatar", ResourceId, AvatarId);

    [BsonIgnore]
    public bool HasAvatar => AvatarId != null;

    [BsonIgnore]
    public bool IsManaged => Account.ManagedAccountTeamId != null;

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!HasAvatar)
        {
            if (UseGravatar)
                return GetGravatarOrDefault();

            return "https://cdn.fluxpoint.dev/devspace/default_avatar." + (usePng ? "png" : "webp");
        }

        return Avatar.Url(usePng ? "png" : "webp");
    }

    public bool UseGravatar { get; set; } = true;

    public string GetGravatarOrDefault()
    {
        string md5Email = MD5.Calculate(System.Text.Encoding.ASCII.GetBytes(Email != null ? Email : ""));

        return $"https://secure.gravatar.com/avatar/{md5Email}?d=mp&s=128";
    }

    public bool IsInstanceAdmin { get; set; }

    public UserAccount Account { get; set; } = new UserAccount();
    public UserSettings Settings { get; set; } = new UserSettings();
    public UserMfa Mfa { get; set; } = new UserMfa();
    public UserDisabled? Disabled { get; set; }

    public void UpdatePartial()
    {
        if (_DB.Users.TryGetValue(Id, out PartialUserData? ud))
            ud.Update(this);
    }
}








[CollectionName("user_roles")]
public class UserRole : MongoIdentityRole<ObjectId>
{

}
