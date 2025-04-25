using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker;

public static class DockerVolumes
{
    public static async Task<IList<DockerVolumeInfo>> ListVolumesAsync(DockerClient client)
    {
        IList<DockerVolumeInfo> List = new List<DockerVolumeInfo>();
        VolumesListResponse Volumes = await client.Volumes.ListAsync();
        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        foreach (VolumeResponse? i in Volumes.Volumes)
        {
            DockerVolumeInfo Volume = DockerVolumeInfo.Create(i, false);
            foreach (ContainerListResponse? c in Containers.Where(x => x.Mounts.Any(x => x.Name == i.Name)))
            {
                Volume.ContainersCount += 1;
                if (c.Names != null && c.Names.Any())
                    Volume.Containers.Add(c.ID, c.Names.First().Substring(1));
                else
                    Volume.Containers.Add(c.ID, c.ID);
            }
            List.Add(Volume);
        }

        return List;
    }

    public static async Task<VolumeResponse> CreateVolumeAsync(DockerClient client, VolumesCreateParameters param)
    {
        return await client.Volumes.CreateAsync(param);
    }

    public static async Task<object?> ControlVolumeAsync(DockerClient client, DockerEvent @event, string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Volume id is missing.");

        switch (@event.VolumeType)
        {
            case ControlVolumeType.View:
                VolumeResponse Volume = await client.Volumes.InspectAsync(id);
                DockerVolumeInfo Data = DockerVolumeInfo.Create(Volume, true);

                IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                                {
                                    { "volume", new Dictionary<string, bool>
                                    {
                                        { id, true }
                                    }
                                    }
                                }
                });

                Data.ContainersCount = containers.Count;
                if (Data.ContainersCount != 0)
                    Data.ContainersList = containers.ToDictionary(x => x.ID, x => DockerMountContainer.Create(x.Mounts.First(x => x.Name == id), x));

                return Data;
            case ControlVolumeType.Remove:
            case ControlVolumeType.ForceRemove:
                await client.Volumes.RemoveAsync(id, @event.VolumeType == ControlVolumeType.ForceRemove);
                break;
        }

        return null;
    }
}
