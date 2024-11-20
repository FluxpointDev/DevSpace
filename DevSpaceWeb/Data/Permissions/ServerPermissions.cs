namespace DevSpaceWeb.Data.Permissions;

public class ServerPermissions
{
    public ServerPermissions(ServerPermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the team.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool ViewServer => Has(ServerPermission.ViewServer);
    public bool ManagePermissions => Has(ServerPermission.ManagePermissions);

    public bool ServerAdministrator => Has(ServerPermission.ServerAdministrator);

    public bool Has(ServerPermission permission)
    {
        if (permission != ServerPermission.ServerAdministrator && Has(ServerPermission.ServerAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
