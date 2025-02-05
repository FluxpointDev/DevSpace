using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerNetworks
    {
        public static async Task<IList<NetworkResponse>> ListNetworksAsync(DockerClient client)
        {
            return await client.Networks.ListNetworksAsync();
        }
    }
}
