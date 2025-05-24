using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerContainerChanges
{
    public string? ContainerName { get; set; }
    public IList<ContainerFileSystemChangeResponse> Changes { get; set; }
}
