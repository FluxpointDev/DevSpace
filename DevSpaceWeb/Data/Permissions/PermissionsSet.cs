namespace DevSpaceWeb.Data.Permissions;

public class PermissionsSet
{
    public static PermissionsSet MaxPermissions = new PermissionsSet
    {
        TeamPermissions = (TeamPermission)ulong.MaxValue,
        APIPermissions = (APIPermission)ulong.MaxValue,
        ConsolePermissions = (ConsolePermission)ulong.MaxValue,
        DockerPermissions = (DockerPermission)ulong.MaxValue,
        DockerContainerPermissions = (DockerContainerPermission)ulong.MaxValue,
        ProjectPermissions = (ProjectPermission)ulong.MaxValue,
        ServerPermissions = (ServerPermission)ulong.MaxValue,
        WebsitePermissions = (WebsitePermission)ulong.MaxValue,
        AppPermissions = (AppPermission)ulong.MaxValue,
    };

    public void AddFrom(PermissionsSet perms)
    {
        ConsolePermissions |= perms.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator)
                ? (ConsolePermission)ulong.MaxValue : perms.ConsolePermissions;

        DockerPermissions |= perms.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || perms.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator)
        ? (DockerPermission)ulong.MaxValue : perms.DockerPermissions;

        DockerContainerPermissions |= perms.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || perms.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator)
        ? (DockerContainerPermission)ulong.MaxValue : perms.DockerContainerPermissions;

        ProjectPermissions |= perms.ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator)
        ? (ProjectPermission)ulong.MaxValue : perms.ProjectPermissions;

        ServerPermissions |= perms.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator)
        ? (ServerPermission)ulong.MaxValue : perms.ServerPermissions;

        TeamPermissions |= perms.TeamPermissions.HasFlag(TeamPermission.TeamAdministrator)
        ? (TeamPermission)ulong.MaxValue : perms.TeamPermissions;

        APIPermissions |= perms.APIPermissions.HasFlag(APIPermission.APIAdministrator)
        ? (APIPermission)ulong.MaxValue : perms.APIPermissions;

        WebsitePermissions |= perms.WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator)
        ? (WebsitePermission)ulong.MaxValue : perms.WebsitePermissions;

        AppPermissions |= perms.AppPermissions.HasFlag(AppPermission.AppAdministrator)
            ? (AppPermission)ulong.MaxValue : perms.AppPermissions;
    }

    public TeamPermission TeamPermissions { get; set; }
    public APIPermission APIPermissions { get; set; }
    public ProjectPermission ProjectPermissions { get; set; }
    public ServerPermission ServerPermissions { get; set; }
    public WebsitePermission WebsitePermissions { get; set; }
    public ConsolePermission ConsolePermissions { get; set; }
    public DockerPermission DockerPermissions { get; set; }
    [Obsolete]
    public WebsitePermission LogPermissions { get; set; }
    public DockerContainerPermission DockerContainerPermissions { get; set; }

    public AppPermission AppPermissions { get; set; }

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        if (checkPermission != TeamPermission.GlobalAdministrator && TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (TeamPermissions.HasFlag(TeamPermission.TeamAdministrator))
            return true;

        if (TeamPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasAPIPermission(APIPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (APIPermissions.HasFlag(APIPermission.APIAdministrator))
            return true;

        if (APIPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasAppPermission(AppPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (AppPermissions.HasFlag(AppPermission.AppAdministrator))
            return true;

        if (AppPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasServerPermission(ServerPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
            return true;

        if (ServerPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasDockerPermission(DockerPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
            return true;

        if (DockerPermissions.HasFlag(DockerPermission.DockerAdministrator))
            return true;

        if (DockerPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasDockerContainerPermission(DockerContainerPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
            return true;

        if (DockerPermissions.HasFlag(DockerPermission.DockerAdministrator))
            return true;

        if (DockerContainerPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasConsolePermission(ConsolePermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator))
            return true;

        if (ConsolePermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasProjectPermission(ProjectPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator))
            return true;

        if (ProjectPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasWebsitePermission(WebsitePermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator))
            return true;

        if (WebsitePermissions.HasFlag(checkPermission))
            return true;

        return false;
    }


    public PermissionsSet Clone()
    {
        return (PermissionsSet)this.MemberwiseClone();
    }
}
