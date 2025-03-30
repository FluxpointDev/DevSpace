using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker;

public static class DockerNetworks
{
    public static async Task<IList<DockerNetworkInfo>> ListNetworksAsync(DockerClient client)
    {
        IList<DockerNetworkInfo> List = new List<DockerNetworkInfo>();
        IList<NetworkResponse> Networks = await client.Networks.ListNetworksAsync();
        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        foreach (NetworkResponse i in Networks)
        {
            DockerNetworkInfo Network = DockerNetworkInfo.Create(i, false);

            foreach (ContainerListResponse? c in Containers.Where(x => x.NetworkSettings.Networks != null && x.NetworkSettings.Networks.ContainsKey(i.Name)))
            {
                Network.ContainersCount += 1;
                if (c.Names != null && c.Names.Any())
                    Network.Containers.Add(c.ID, c.Names.First().Substring(1));
                else
                    Network.Containers.Add(c.ID, c.ID);
            }
            List.Add(Network);
        }
        return List;
    }

    public static async Task<object?> ControlNetworkAsync(DockerClient client, DockerEvent @event, string id)
    {
        switch (@event.NetworkType)
        {
            case ControlNetworkType.View:
                NetworkResponse Network = await client.Networks.InspectNetworkAsync(id);
                return DockerNetworkInfo.Create(Network, true);
            case ControlNetworkType.Remove:
                await client.Networks.DeleteNetworkAsync(id);
                break;
            case ControlNetworkType.LeaveNetwork:
                await client.Networks.DisconnectNetworkAsync(id, new NetworkDisconnectParameters
                {
                    Container = @event.ResourceList.First()
                });
                break;
        }
        return null;
    }
}
