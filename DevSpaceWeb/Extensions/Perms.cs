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
            if (!Client.HasConsolePermission(console, value))
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
            if (!Client.HasTeamPermission(value))
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
            if (!Client.HasServerPermission(server, value))
            {
                perm = value;
                return true;
            }
        }
        perm = null;
        return false;
    }

    public static bool CheckDockerManagerPermission(DockerPermission perm)
    {
        switch (perm)
        {
            case DockerPermission.ContainerConsole:
            case DockerPermission.ContainerInspect:
            case DockerPermission.ContainerLogs:
            case DockerPermission.ContainerStats:
            case DockerPermission.ControlContainers:
            case DockerPermission.ManageContainers:
            case DockerPermission.ManageCustomTemplates:
            case DockerPermission.ManageImages:
            case DockerPermission.ManageNetworks:
            case DockerPermission.ManageRegistries:
            case DockerPermission.ManageSettings:
            case DockerPermission.ManageStackPermissions:
            case DockerPermission.ManageVolumes:
            case DockerPermission.ViewContainers:
            case DockerPermission.ViewCustomTemplates:
            case DockerPermission.ViewEvents:
            case DockerPermission.ViewImages:
            case DockerPermission.ViewNetworks:
            case DockerPermission.ViewPlugins:
            case DockerPermission.ViewRegistries:
            case DockerPermission.ViewTemplates:
            case DockerPermission.ViewVolumes:
                return true;
        }

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
