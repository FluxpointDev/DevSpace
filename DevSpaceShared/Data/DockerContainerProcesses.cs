using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerContainerProcesses
{
    public string ContainerName { get; set; }
    public ContainerProcessesResponse Data { get; set; }

    public bool IsNotRunning { get; set; }
}
