
using Docker.DotNet.Models;

namespace DevSpaceShared.Data;

public class DockerStackInfo
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long Version { get; set; }
    public DockerStackType Type { get; set; }
    public DockerStackControl ControlType { get; set; }
    public HashSet<DockerContainerInfo> Containers { get; set; } = new HashSet<DockerContainerInfo>();
    public HashSet<MountPoint> Volumes { get; set; } = new HashSet<MountPoint>();
    public HashSet<DockerContainerNetwork> Networks { get; set; } = new HashSet<DockerContainerNetwork>();
    public int ContainersCount { get; set; }
    public bool IsRunning { get; set; } = false;
}
public enum DockerStackType
{
    Compose
}
public enum DockerStackControl
{
    Full, Portainer, System
}
