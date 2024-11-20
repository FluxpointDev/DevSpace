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

    public bool LogAdministrator => Has(LogPermission.LogAdministrator);

    public bool Has(LogPermission permission)
    {
        if (permission != LogPermission.LogAdministrator && Has(LogPermission.LogAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}