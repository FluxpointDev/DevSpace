namespace DevSpaceWeb.Data.Permissions;

public class WebsitePermissions
{
    public WebsitePermissions(WebsitePermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the team.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool ViewWebsite => Has(WebsitePermission.ViewWebsite);
    public bool ManagePermissions => Has(WebsitePermission.ManagePermissions);

    public bool WebsiteAdministrator => Has(WebsitePermission.WebsiteAdministrator);

    public bool Has(WebsitePermission permission)
    {
        if (permission != WebsitePermission.WebsiteAdministrator && Has(WebsitePermission.WebsiteAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
