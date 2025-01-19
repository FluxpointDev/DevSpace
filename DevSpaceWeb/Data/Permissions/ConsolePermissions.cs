namespace DevSpaceWeb.Data.Permissions;

public class ConsolePermissions
{
    public ConsolePermissions(ConsolePermission permissions)
    {
        Raw = (ulong)permissions;
    }

    /// <summary>
    /// Raw permissions number for the log.
    /// </summary>
    public ulong Raw { get; internal set; }

    public bool ConsoleAdministrator => Has(ConsolePermission.ConsoleAdministrator);

    public bool Has(ConsolePermission permission)
    {
        if (permission != ConsolePermission.ConsoleAdministrator && Has(ConsolePermission.ConsoleAdministrator))
            return true;

        ulong Flag = (ulong)permission;
        return (Raw & Flag) == Flag;
    }
}
