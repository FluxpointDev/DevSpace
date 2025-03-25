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

    public string id;
    public string team_id;
    public string name;
    public string description;
    public int position;
    public PermissionsSet? permissions;
    public DateTime created_at;
}
