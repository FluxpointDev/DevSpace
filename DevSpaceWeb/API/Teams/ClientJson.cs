using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Permissions;

namespace DevSpaceWeb.API.Teams;

public class ClientJson
{
    public ClientJson(APIClient data)
    {
        id = data.Id.ToString();
        name = data.Name;
        owner_id = data.OwnerId.ToString();
        team_id = data.TeamId.ToString();
        is_disabled = data.IsDisabled;
        created_at = data.CreatedAt;
        token_generated_at = data.GeneratedAt;
        use_custom_permissions = data.UseCustomPermissions;
        if (data.UseCustomPermissions)
            custom_permissions = data.CustomPermissions;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string owner_id { get; set; }
    public string team_id { get; set; }
    public bool is_disabled { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? token_generated_at { get; set; }
    public bool use_custom_permissions { get; set; }
    public PermissionsSet? custom_permissions { get; set; }
}
