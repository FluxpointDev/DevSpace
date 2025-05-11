using Docker.DotNet.Models;

namespace DevSpaceShared;
public static class DockerExtensions
{
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
}
