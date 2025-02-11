namespace DevSpaceWeb.Data.Permissions;

public class PermissionsSet
{
    public static PermissionsSet MaxPermissions = new PermissionsSet
    {
        TeamPermissions = (TeamPermission)ulong.MaxValue,
        APIPermissions = (APIPermission)ulong.MaxValue,
        ConsolePermissions = (ConsolePermission)ulong.MaxValue,
        DockerPermissions = (DockerPermission)ulong.MaxValue,
        LogPermissions = (LogPermission)ulong.MaxValue,
        ProjectPermissions = (ProjectPermission)ulong.MaxValue,
        ServerPermissions = (ServerPermission)ulong.MaxValue,
        WebsitePermissions = (WebsitePermission)ulong.MaxValue
    };

    public void AddFrom(PermissionsSet perms)
    {
        ConsolePermissions |= perms.ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator)
                ? (ConsolePermission)ulong.MaxValue : perms.ConsolePermissions;

        DockerPermissions |= perms.ServerPermissions.HasFlag(ServerPermission.ServerAdministrator) || perms.DockerPermissions.HasFlag(DockerPermission.DockerAdministrator)
        ? (DockerPermission)ulong.MaxValue : perms.DockerPermissions;

        LogPermissions |= perms.LogPermissions.HasFlag(LogPermission.LogAdministrator)
        ? (LogPermission)ulong.MaxValue : perms.LogPermissions;

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
    }

    public TeamPermission TeamPermissions;
    public APIPermission APIPermissions;
    public LogPermission LogPermissions;
    public ProjectPermission ProjectPermissions;
    public ServerPermission ServerPermissions;
    public WebsitePermission WebsitePermissions;
    public ConsolePermission ConsolePermissions;
    public DockerPermission DockerPermissions;

    public bool HasTeamPermission(TeamPermission checkPermission)
    {
        if (checkPermission != TeamPermission.GlobalAdministrator && TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != TeamPermission.TeamAdministrator && TeamPermissions.HasFlag(TeamPermission.TeamAdministrator))
            return true;

        if (TeamPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasAPIPermission(APIPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != APIPermission.APIAdministrator && APIPermissions.HasFlag(APIPermission.APIAdministrator))
            return true;

        if (APIPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasServerPermission(ServerPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != ServerPermission.ServerAdministrator && ServerPermissions.HasFlag(ServerPermission.ServerAdministrator))
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

        if (checkPermission != DockerPermission.DockerAdministrator)
            return true;

        if (DockerPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasConsolePermission(ConsolePermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != ConsolePermission.ConsoleAdministrator && ConsolePermissions.HasFlag(ConsolePermission.ConsoleAdministrator))
            return true;

        if (ConsolePermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasLogPermission(LogPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != LogPermission.LogAdministrator && LogPermissions.HasFlag(LogPermission.LogAdministrator))
            return true;

        if (LogPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasProjectPermission(ProjectPermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != ProjectPermission.ProjectAdministrator && ProjectPermissions.HasFlag(ProjectPermission.ProjectAdministrator))
            return true;

        if (ProjectPermissions.HasFlag(checkPermission))
            return true;

        return false;
    }

    public bool HasWebsitePermission(WebsitePermission checkPermission)
    {
        if (TeamPermissions.HasFlag(TeamPermission.GlobalAdministrator))
            return true;

        if (checkPermission != WebsitePermission.WebsiteAdministrator && WebsitePermissions.HasFlag(WebsitePermission.WebsiteAdministrator))
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
