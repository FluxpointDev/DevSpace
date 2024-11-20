namespace DevSpaceWeb.Data.Permissions;

public class DockerPermissions
{
    public DockerPermissions(DockerPermission permissions)
    {
        Raw = (ulong)permissions;
    }

    public ulong Raw { get; internal set; }

    public bool DockerAdministrator => Has(DockerPermission.DockerAdministrator);

    public bool Has(DockerPermission permission)
    {
        if (permission != DockerPermission.DockerAdministrator && Has(DockerPermission.DockerAdministrator))
            return true;
        if (permission != DockerPermission.DockerAdministrator && permission != DockerPermission.DockerManager && Has(DockerPermission.DockerManager))
        {
            // Manager permissions
            switch (permission)
            {
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
                    return true;
            }
        }

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
