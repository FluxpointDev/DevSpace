namespace DevSpaceWeb.Data.Permissions;

public class PermissionsSet
{
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

        if (checkPermission != APIPermission.APIAdministrator && TeamPermissions.HasFlag(APIPermission.APIAdministrator))
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

        // Docker manager permissions set
        switch (checkPermission)
        {
            case DockerPermission.ContainerConsole:
            case DockerPermission.ContainerInspect:
            case DockerPermission.ContainerLogs:
            case DockerPermission.ContainerStats:
            case DockerPermission.ControlContainers:
            case DockerPermission.ManageContainers:
            case DockerPermission.ManageCustomTemplates:
            case DockerPermission.ManageImages:
            case DockerPermission.ManageNetworks:
            case DockerPermission.ManageRegistries:
            case DockerPermission.ManageSettings:
            case DockerPermission.ManageStackPermissions:
            case DockerPermission.ManageVolumes:
            case DockerPermission.ViewContainers:
            case DockerPermission.ViewCustomTemplates:
            case DockerPermission.ViewEvents:
            case DockerPermission.ViewImages:
            case DockerPermission.ViewNetworks:
            case DockerPermission.ViewPlugins:
            case DockerPermission.ViewRegistries:
            case DockerPermission.ViewTemplates:
            case DockerPermission.ViewVolumes:
                {
                    if (checkPermission != DockerPermission.DockerManager && DockerPermissions.HasFlag(DockerPermission.DockerManager))
                        return true;
                }
                break;
        }

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
