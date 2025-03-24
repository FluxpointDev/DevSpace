using DevSpaceWeb.Data.Users;

namespace DevSpaceWeb.API.Users;

public class UserJson
{
    public UserJson(AuthUser data)
    {
        id = data.Id.ToString();
        username = data.UserName!;
        displayname = data.DisplayName;
        created_at = data.CreatedOn;
        if (data.AvatarId.HasValue)
            avatar_url = data.GetAvatarOrDefault();
    }

    public string id { get; set; }
    public string username { get; set; }
    public string? displayname { get; set; }
    public string? avatar_url { get; set; }
    public string? background_url { get; set; }
    public bool is_managed { get; set; }
    public DateTime created_at { get; set; }
}
