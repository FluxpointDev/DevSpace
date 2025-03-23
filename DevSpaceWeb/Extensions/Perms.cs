using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Servers;

namespace DevSpaceWeb.Extensions;

public static class Perms
{
    public static bool CheckFailedConsolePermissions(this APIClient Client, ConsoleData console, ConsolePermission flags, out ConsolePermission? perm)
    {
        foreach (ConsolePermission value in GetUniqueFlags(flags))
        {
            if (!Client.HasConsolePermission(console.Team, console, value))
            {
                perm = value;
                return true;
            }
        }
        perm = null;
        return false;
    }

    public static bool CheckFailedTeamPermissions(this APIClient Client, TeamPermission flags, out TeamPermission? perm)
    {
        foreach (TeamPermission value in GetUniqueFlags(flags))
        {
            if (!Client.HasTeamPermission(Client.Team, value))
            {
                perm = value;
                return true;
            }
        }
        perm = null;
        return false;
    }

    public static bool CheckFailedServerPermissions(this APIClient Client, ServerData server, ServerPermission flags, out ServerPermission? perm)
    {
        foreach (ServerPermission value in GetUniqueFlags(flags))
        {
            if (!Client.HasServerPermission(server.Team, server, value))
            {
                perm = value;
                return true;
            }
        }
        perm = null;
        return false;
    }
    public static IEnumerable<T> GetUniqueFlags<T>(T flags) where T : Enum
    {
        ulong flag = 1;
        foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
        {
            ulong bits = Convert.ToUInt64(value);
            while (flag < bits)
            {
                flag <<= 1;
            }

            if (flag == bits && flags.HasFlag(value))
            {
                yield return value;
            }
        }
    }


}
