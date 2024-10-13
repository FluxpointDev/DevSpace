
using MongoDB.Bson;

namespace DevSpaceWeb.Data;

public class PartialUserData
{
    public PartialUserData(AuthUser user)
    {
        Update(user);
    }

    public ObjectId Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public Guid? AvatarId { get; set; }

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!AvatarId.HasValue)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return _Data.Config.Instance.GetPublicUrl() + "/public/resources/" + ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";
    }
    public Guid? ResourceId { get; set; }
    public ObjectId? ManagedAccountTeamId { get; set; }

    public void Update(AuthUser user)
    {
        Id = user.Id;
        UserName = user.UserName;
        DisplayName = user.DisplayName;
        Email = user.Email;
        AvatarId = user.AvatarId;
        ResourceId = user.ResourceId;
        ManagedAccountTeamId = user.ManagedAccountTeamId;
    }
}
