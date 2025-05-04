using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class TeamJson
{
    public TeamJson(TeamData data, bool viewPermissions)
    {
        id = data.Id.ToString();
        name = data.Name;
        created_at = data.CreatedAt;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        if (data.IconId.HasValue)
            icon_url = data.GetIconOrDefault();
        if (viewPermissions)
            default_permissions = data.DefaultPermissions;
        default_members_color = data.DefaultMembersColor;
        require_2fa = data.Require2FA;
        roles = data.CachedRoles.Values.Select(x => x.Id.ToString()).ToArray();
    }

    public string id;
    public string name;
    public DateTime created_at;
    public string owner_id;
    public string? vanity_url;
    public string? icon_url;
    public PermissionsSet? default_permissions;
    public string? default_members_color;
    public bool require_2fa;
    public string[] roles;
}
