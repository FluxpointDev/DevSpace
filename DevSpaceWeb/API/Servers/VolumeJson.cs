using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class VolumeJson
{
    public VolumeJson(DockerVolumeInfo volume)
    {
        name = volume.Name;
        created_at = volume.CreatedAt;
        driver = volume.Driver;
        scope = volume.Scope;
        mountpoint = volume.MountPoint;

        if (volume.Status != null)
            status = volume.Status;
        else
            status = new Dictionary<string, object>();

        if (volume.Labels != null)
            labels = volume.Labels;
        else
            labels = new Dictionary<string, string>();

        if (volume.Options != null)
            options = volume.Options;
        else
            options = new Dictionary<string, string>();

        if (volume.ContainersList != null)
            containers = volume.ContainersList.ToDictionary(x => x.Key, x => new VolumeContainerJson(x.Value));
        else
            containers = [];

        if (volume.Containers != null)
            containers_list = volume.Containers;
        else
            containers_list = [];

        containers_count = volume.ContainersCount;
    }

    public string? name { get; set; }
    public string? created_at { get; set; }
    public string? driver { get; set; }
    public string? scope { get; set; }
    public string? mountpoint { get; set; }
    public IDictionary<string, object> status { get; set; }
    public IDictionary<string, string> labels { get; set; }
    public IDictionary<string, string> options { get; set; }
    public Dictionary<string, VolumeContainerJson> containers { get; set; }
    public Dictionary<string, string> containers_list { get; set; }
    public int containers_count { get; set; }
}
public class VolumeContainerJson
{
    public VolumeContainerJson(DockerMountContainer container)
    {
        name = container.Name;
        mounted_at = container.Mounted;
        read_write = container.ReadWrite;
    }

    public string name { get; set; }
    public string mounted_at { get; set; }
    public bool read_write { get; set; }
}