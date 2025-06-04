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
        if (volume.Labels != null && volume.Labels.TryGetValue("com.docker.compose.project", out string? project))
            Info.Stack = project;

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
    public Dictionary<string, string> Containers { get; set; } = [];
    public Dictionary<string, DockerMountContainer> ContainersList { get; set; } = [];
    public int ContainersCount { get; set; }
    public string Stack { get; set; }
}
