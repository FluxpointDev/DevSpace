
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data;

public class PartialUserData
{
    public PartialUserData(AuthUser user)
    {
        Update(user);
    }

    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public Guid? AvatarId { get; set; }
    public Guid? ResourceId { get; set; }

    public void Update(AuthUser user)
    {
        UserName = user.UserName;
        DisplayName = user.DisplayName;
        Email = user.Email;
        AvatarId = user.AvatarId;
        ResourceId = user.ResourceId;
    }
}
