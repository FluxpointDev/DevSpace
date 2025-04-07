using Docker.DotNet.Models;

namespace DevSpaceShared.Data;

public class DockerVolumeInfo
{
    public static DockerVolumeInfo Create(VolumeResponse volume, bool inspect)
    {
        DockerVolumeInfo Info = new DockerVolumeInfo
        {
            Name = volume.Name,
            CreatedAt = volume.CreatedAt,
            Driver = volume.Driver,
            Scope = volume.Scope,
            MountPoint = volume.Mountpoint
        };

        if (inspect)
        {
            Info.Status = volume.Status;
            Info.Labels = volume.Labels;
            Info.Options = volume.Options;
        }

        return Info;
    }

    public string? Name { get; set; }
    public string? CreatedAt { get; set; }
    public string? Driver { get; set; }
    public string? Scope { get; set; }
    public string? MountPoint { get; set; }
    public IDictionary<string, object>? Status { get; set; }
    public IDictionary<string, string>? Labels { get; set; }
    public IDictionary<string, string>? Options { get; set; }
    public Dictionary<string, string> Containers { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, DockerMountContainer> ContainersList { get; set; } = new Dictionary<string, DockerMountContainer>();
    public int ContainersCount { get; set; }
}
