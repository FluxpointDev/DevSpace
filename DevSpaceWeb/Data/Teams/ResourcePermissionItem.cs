using DevSpaceWeb.Data.Permissions;

namespace DevSpaceWeb.Data.Teams;

public class ResourcePermissionItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public PermissionsSet Permissions { get; set; }
}
