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
        is_disabled = data.IsDisabled;
        created_at = data.CreatedAt;
        token_generated_at = data.GeneratedAt;
        allowed_permissions = data.AllowedPermissions;
        use_custom_permissions = data.UseCustomPermissions;
        if (data.UseCustomPermissions)
            custom_permissions = data.CustomPermissions;
    }

    public string id;
    public string name;
    public string owner_id;
    public bool is_disabled;
    public DateTime created_at;
    public DateTime? token_generated_at;
    public bool use_custom_permissions;
    public PermissionsSet allowed_permissions;
    public PermissionsSet? custom_permissions;
}
