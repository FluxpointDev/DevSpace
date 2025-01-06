using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class RoleJson
{
    public RoleJson(TeamRoleData data)
    {
        id = data.Id.ToString();
        team_id = data.TeamId.ToString();
        name = data.Name;
        description = data.Description;
        position = data.Position;
        permissions = data.Permissions;
    }

    public string id { get; set; }
    public string team_id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int position { get; set; }
    public PermissionsSet permissions { get; set; }
}
