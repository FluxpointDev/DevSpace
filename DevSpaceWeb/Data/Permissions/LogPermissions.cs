namespace DevSpaceWeb.Data.Permissions;

public class LogPermissions
{
    public LogPermissions(LogPermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the log.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool ViewLog => Has(LogPermission.ViewLog);
    public bool ManagePermissions => Has(LogPermission.ManagePermissions);

    public bool LogAdministrator => Has(LogPermission.LogAdministrator);

    public bool Has(LogPermission permission)
    {
        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}