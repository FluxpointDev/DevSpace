using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class TeamJson
{
    public TeamJson(TeamData data)
    {
        id = data.Id.ToString();
        name = data.Name;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        if (data.IconId.HasValue)
            icon_url = data.GetIconOrDefault();
        default_permissions = data.DefaultPermissions;
        roles = data.CachedRoles.Values.Select(x => x.Id.ToString()).ToArray();
        members = data.Members.ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
    }

    public string id { get; set; }
    public string name { get; set; }
    public string owner_id { get; set; }
    public string vanity_url { get; set; }
    public string icon_url { get; set; }
    public PermissionsSet default_permissions { get; set; }
    public string[] roles { get; set; }
    public Dictionary<string, string> members { get; set; }
}
