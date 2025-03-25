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
        mount_point = volume.MountPoint;
    }

    public string name { get; set; }
    public string created_at { get; set; }
    public string driver { get; set; }
    public string scope { get; set; }
    public string mount_point { get; set; }
}
