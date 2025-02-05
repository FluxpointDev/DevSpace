using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerSystem
    {
        public static async Task<SystemInfoResponse> GetSystemInfoAsync(DockerClient client)
        {
            return await client.System.GetSystemInfoAsync();
        }
    }
}
