using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb;

public static class Permissions
{
    public static bool CheckFailedTeamPermissions(TeamMemberData? member, TeamData? team, TeamPermission flags, out TeamPermission? failedPerm)
    {
        foreach (TeamPermission value in GetUniqueFlags(flags))
        {
            if (member == null || !member.HasTeamPermission(team, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedConsolePermissions(TeamMemberData? member, TeamData? team, ConsoleData? console, ConsolePermission flags, out ConsolePermission? failedPerm)
    {
        foreach (ConsolePermission value in GetUniqueFlags(flags))
        {
            if (member == null || !member.HasConsolePermission(team, console, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedServerPermissions(TeamMemberData? member, TeamData? team, ServerData? server, ServerPermission flags, out ServerPermission? failedPerm)
    {
        foreach (ServerPermission value in GetUniqueFlags(flags))
        {
            if (member == null || !member.HasServerPermission(team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedDockerPermissions(TeamMemberData? member, TeamData? team, ServerData? server, DockerPermission flags, out DockerPermission? failedPerm)
    {
        foreach (DockerPermission value in GetUniqueFlags(flags))
        {
            if (member == null || !member.HasDockerPermission(team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedDockerContainerPermissions(TeamMemberData? member, TeamData? team, ServerData? server, DockerContainerPermission flags, out DockerContainerPermission? failedPerm)
    {
        foreach (DockerContainerPermission value in GetUniqueFlags(flags))
        {
            if (member == null || !member.HasDockerContainerPermission(team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedTeamPermissions(this APIClient client, TeamPermission flags, out TeamPermission? failedPerm)
    {
        foreach (TeamPermission value in GetUniqueFlags(flags))
        {
            if (!client.HasTeamPermission(client.Team, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedConsolePermissions(this APIClient client, ConsoleData? console, ConsolePermission flags, out ConsolePermission? failedPerm)
    {
        foreach (ConsolePermission value in GetUniqueFlags(flags))
        {
            if (console == null || !client.HasConsolePermission(console.Team, console, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedServerPermissions(this APIClient client, ServerData? server, ServerPermission flags, out ServerPermission? failedPerm)
    {
        foreach (ServerPermission value in GetUniqueFlags(flags))
        {
            if (server == null || !client.HasServerPermission(server.Team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedDockerPermissions(this APIClient client, ServerData? server, DockerPermission flags, out DockerPermission? failedPerm)
    {
        foreach (DockerPermission value in GetUniqueFlags(flags))
        {
            if (server == null || !client.HasDockerPermission(server.Team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    public static bool CheckFailedDockerContainerPermissions(this APIClient client, ServerData? server, DockerContainerPermission flags, out DockerContainerPermission? failedPerm)
    {
        foreach (DockerContainerPermission value in GetUniqueFlags(flags))
        {
            if (server == null || !client.HasDockerContainerPermission(server.Team, server, value))
            {
                failedPerm = value;
                return true;
            }
        }
        failedPerm = null;
        return false;
    }

    private static IEnumerable<T> GetUniqueFlags<T>(T flags) where T : Enum
    {
        ulong flag = 1;
        foreach (T value in Enum.GetValues(flags.GetType()).Cast<T>())
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
