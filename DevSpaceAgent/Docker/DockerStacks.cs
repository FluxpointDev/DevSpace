using DevSpaceShared.Data;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerStacks
    {
        public static async Task<List<DockerStackInfo>> ListStacksAsync(DockerClient client)
        {
            List<DockerStackInfo> Stacks = new List<DockerStackInfo>();
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                Size = true,
                All = true
            });

            foreach (ContainerListResponse? c in containers)
            {
                if (!c.Labels.TryGetValue("com.docker.compose.project", out string label))
                    continue;

                DockerStackInfo? Stack = Stacks.FirstOrDefault(x => x.Name == label);
                if (Stack != null)
                    Stack.Containers.Add(c.Names.First().Substring(1));
            }

            return Stacks;
        }
    }
}
