namespace DevSpaceWeb.Data.Permissions;

public class TeamPermissions
{
    public TeamPermissions(TeamPermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the team.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool GlobalAdministrator => Has(TeamPermission.GlobalAdministrator);

    public bool Has(TeamPermission permission)
    {
        if (permission != TeamPermission.GlobalAdministrator && Has(TeamPermission.GlobalAdministrator))
            return true;

        if (permission != TeamPermission.TeamAdministrator && Has(TeamPermission.TeamAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}