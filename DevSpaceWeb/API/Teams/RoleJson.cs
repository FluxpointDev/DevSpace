using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class RoleJson
{
    public RoleJson(TeamRoleData data, bool viewPermissions)
    {
        id = data.Id.ToString();
        team_id = data.TeamId.ToString();
        created_at = data.CreatedAt;
        name = data.Name;
        description = data.Description;
        position = data.GetPosition();
        if (viewPermissions)
            permissions = data.Permissions;
    }

    public string id { get; set; }
    public string team_id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int position { get; set; }
    public PermissionsSet? permissions { get; set; }
    public DateTime created_at { get; set; }
}
