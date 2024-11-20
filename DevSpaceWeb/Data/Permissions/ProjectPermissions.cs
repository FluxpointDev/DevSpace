namespace DevSpaceWeb.Data.Permissions;

public class ProjectPermissions
{
    public ProjectPermissions(ProjectPermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the team.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool ViewProject => Has(ProjectPermission.ViewProject);
    public bool ManagePermissions => Has(ProjectPermission.ManagePermissions);

    public bool ProjectAdministrator => Has(ProjectPermission.ProjectAdministrator);

    public bool Has(ProjectPermission permission)
    {
        if (permission != ProjectPermission.ProjectAdministrator && Has(ProjectPermission.ProjectAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
