using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerMountContainer
{
    public static DockerMountContainer Create(MountPoint mount, ContainerListResponse x)
    {
        return new DockerMountContainer
        {
            Name = x.Names.First().Substring(1),
            Mounted = mount.Destination,
            ReadWrite = mount.RW
        };
    }
    public string Name { get; set; }
    public string Mounted { get; set; }
    public bool ReadWrite { get; set; }
}
