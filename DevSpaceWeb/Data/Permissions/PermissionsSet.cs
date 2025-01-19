namespace DevSpaceWeb.Data.Permissions;

public class PermissionsSet
{
    public TeamPermission TeamPermissions;
    public LogPermission LogPermissions;
    public ProjectPermission ProjectPermissions;
    public ServerPermission ServerPermissions;
    public WebsitePermission WebsitePermissions;
    public ConsolePermission ConsolePermissions;
    public DockerPermission DockerPermissions;

    public PermissionsSet Clone()
    {
        return (PermissionsSet)this.MemberwiseClone();
    }
}
