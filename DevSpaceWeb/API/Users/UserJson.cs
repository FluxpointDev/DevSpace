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

    public string id;
    public string username;
    public string? displayname;
    public string? avatar_url;
    public string? background_url;
    public bool is_managed;
    public DateTime created_at;
}
