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

        foreach (var i in Volumes.Volumes)
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

    public static async Task<object?> ControlVolumeAsync(DockerClient client, DockerEvent @event, string id)
    {
        switch (@event.VolumeType)
        {
            case ControlVolumeType.View:
                var Volume = await client.Volumes.InspectAsync(id);
                return DockerVolumeInfo.Create(Volume, true);
            case ControlVolumeType.Remove:
            case ControlVolumeType.ForceRemove:
                await client.Volumes.RemoveAsync(id, @event.VolumeType == ControlVolumeType.ForceRemove);
                break;
        }

        return null;
    }
}
