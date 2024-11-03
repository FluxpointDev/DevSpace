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
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
