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

    public bool ViewMembers => Has(TeamPermission.ViewMembers);

    public bool ManageMembers => Has(TeamPermission.ManageMembers);

    public bool ManageRoles => Has(TeamPermission.ManageRoles);

    public bool ManagePermissions => Has(TeamPermission.ManagePermissions);

    public bool ManageTeam => Has(TeamPermission.ManageTeam);

    public bool TeamAdministrator => Has(TeamPermission.TeamAdministrator);

    public bool GlobalAdministrator => Has(TeamPermission.GlobalAdministrator);

    public bool Has(TeamPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}