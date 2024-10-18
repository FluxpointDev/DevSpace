namespace DevSpaceWeb.Data.Permissions;

public class PermissionsSet
{
    public TeamPermission TeamPermissions { get; set; }
    public LogPermission LogPermissions { get; set; }
    public ProjectPermission ProjectPermissions { get; set; }
    public ServerPermission ServerPermissions { get; set; }
    public WebsitePermission WebsitePermissions { get; set; }
}
