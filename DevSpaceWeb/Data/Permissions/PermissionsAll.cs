namespace DevSpaceWeb.Data.Permissions;

public class PermissionsAll
{
    public PermissionsAll(PermissionsSet permissions)
    {
        Team = new TeamPermissions(permissions.TeamPermissions);
        Log = new LogPermissions(permissions.LogPermissions);
        Project = new ProjectPermissions(permissions.ProjectPermissions);
        Server = new ServerPermissions(permissions.ServerPermissions);
        Website = new WebsitePermissions(permissions.WebsitePermissions);
    }

    public TeamPermissions Team { get; set; }
    public LogPermissions Log { get; set; }
    public ProjectPermissions Project { get; set; }
    public ServerPermissions Server { get; set; }
    public WebsitePermissions Website { get; set; }
}
