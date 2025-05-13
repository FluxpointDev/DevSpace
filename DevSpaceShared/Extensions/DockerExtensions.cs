using DevSpaceShared.Data;
using Docker.DotNet.Models;

namespace DevSpaceShared;
public static class DockerExtensions
{
    public static bool IsRunning(this DockerContainerInfo container)
    {
        switch (container.State)
        {
            case "running":
            case "restarting":
            case "paused":
            case "healthy":
                return true;
        }

        return false;
    }

    public static bool IsRunning(this ContainerListResponse container)
    {
        switch (container.State)
        {
            case "running":
            case "restarting":
            case "paused":
            case "healthy":
                return true;
        }

        return false;
    }

    public static bool IsNetworkSystem(this DockerContainerNetwork network)
    {
        return IsNetworkSystem(network.Name);
    }

    public static bool IsNetworkSystem(this DockerNetworkInfo network)
    {
        return IsNetworkSystem(network.Name);
    }

    public static bool IsNetworkSystem(string? name)
    {
        switch (name)
        {
            case "host":
            case "bridge":
            case "none":
                return true;
        }

        return false;
    }
}
