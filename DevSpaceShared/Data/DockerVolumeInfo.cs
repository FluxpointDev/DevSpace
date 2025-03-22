using Docker.DotNet.Models;

namespace DevSpaceShared.Data
{
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

            }

            return Info;
        }

        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string Driver { get; set; }
        public string Scope { get; set; }
        public string MountPoint { get; set; }
        public Dictionary<string, string> Containers { get; set; } = new Dictionary<string, string>();
        public int ContainersCount { get; set; }
    }
}
