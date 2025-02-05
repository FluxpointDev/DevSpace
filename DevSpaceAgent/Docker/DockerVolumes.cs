using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerVolumes
    {
        public static async Task<VolumesListResponse> ListVolumesAsync(DockerClient client)
        {
            return await client.Volumes.ListAsync();
        }
    }
}
