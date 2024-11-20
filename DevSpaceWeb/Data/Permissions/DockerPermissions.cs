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
        ulong AdminFlag = (ulong)DockerPermission.DockerAdministrator;
        if ((Raw & AdminFlag) == AdminFlag)
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
