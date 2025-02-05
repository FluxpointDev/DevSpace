using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerImages
    {
        public static async Task<IList<ImagesListResponse>> ListImagesAsync(DockerClient client)
        {
            return await client.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            });
        }

        public static async Task<IList<ImageSearchResponse>> SearchImagesAsync(DockerClient client, DockerEvent @event)
        {
            return await client.Images.SearchImagesAsync(new ImagesSearchParameters
            {
                Term = @event.ResourceId,
                Limit = 15
            });
        }

        public static async Task<ImagesPruneResponse> PruneImagesAsync(DockerClient client)
        {
            return await client.Images.PruneImagesAsync(new ImagesPruneParameters
            {

            });
        }

        public static async Task<object?> ControlImageAsync(DockerClient client, DockerEvent @event)
        {
            switch (@event.ImageType)
            {
                case ControlImageType.Export:

                    break;
                case ControlImageType.Remove:
                case ControlImageType.RemoveForce:
                    {
                        if (@event.ResourceList != null)
                        {
                            foreach (string r in @event.ResourceList)
                            {
                                await client.Images.DeleteImageAsync(r, new ImageDeleteParameters
                                {
                                    Force = @event.ImageType == ControlImageType.RemoveForce
                                });
                            }
                        }
                        else
                        {
                            await client.Images.DeleteImageAsync(@event.ResourceId, new ImageDeleteParameters
                            {
                                Force = @event.ImageType == ControlImageType.RemoveForce
                            });
                        }
                    }
                    break;
                case ControlImageType.Inspect:
                    return await client.Images.InspectImageAsync(@event.ResourceId);
            }

            return null;
        }
    }
}
