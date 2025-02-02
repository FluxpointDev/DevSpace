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
        roles = data.CachedRoles.Values.Select(x => x.Id.ToString()).ToArray();
    }

    public string id { get; set; }
    public string name { get; set; }
    public DateTime created_at { get; set; }
    public string owner_id { get; set; }
    public string vanity_url { get; set; }
    public string icon_url { get; set; }
    public PermissionsSet? default_permissions { get; set; }
    public string[] roles { get; set; }
}
