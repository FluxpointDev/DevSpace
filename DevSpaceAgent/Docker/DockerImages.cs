using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using System.Text;

namespace DevSpaceAgent.Docker;

public static class DockerImages
{
    public static async Task<IList<DockerImageInfo>> ListImagesAsync(DockerClient client)
    {
        IList<ImagesListResponse> List = await client.Images.ListImagesAsync(new ImagesListParameters
        {
            All = true
        });

        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });
        List<DockerImageInfo> Response = new List<DockerImageInfo>();
        HashSet<string> ParentMap = new HashSet<string>();
        foreach (ImagesListResponse i in List)
        {
            DockerImageInfo Image = DockerImageInfo.Create(i);

            foreach (ContainerListResponse? c in Containers.Where(x => x.ImageID == i.ID))
            {
                Image.ContainersCount += 1;
                if (c.Names != null && c.Names.Any())
                    Image.Containers.Add(c.ID, c.Names.First().Substring(1));
                else
                    Image.Containers.Add(c.ID, c.ID);
            }
            Response.Add(Image);
            if (!string.IsNullOrEmpty(i.ParentID))
            {
                ParentMap.Add(i.ParentID);
            }
        }

        foreach (var i in Response)
        {
            if (ParentMap.Contains(i.ID))
                i.IsParent = true;
        }
        return Response;
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

    public static async Task PullImageAsync(DockerClient client, string name)
    {
        string? TagName = null;
        string[] split = name.Split(':');
        string ImageName = split[0];
        if (split.Length > 1)
            TagName = split[1];

        await client.Images.CreateImageAsync(new ImagesCreateParameters
        {
            FromImage = ImageName,
            Tag = TagName
        }, null, new Progress<JSONMessage>());

    }

    public static async Task CreateImageAsync(DockerClient client, CreateImageEvent build)
    {
        switch (build.Type)
        {
            case CreateImageType.Editor:
                {
                    string TarFile = Program.CurrentDirectory + "Data/Temp/" + Guid.NewGuid().ToString() + ".tar";

                    try
                    {
                        FileStream tarball = new FileStream(TarFile, FileMode.Create);
                        using (TarOutputStream tarArchive = new TarOutputStream(tarball))
                        {
                            TarEntry tarEntry = TarEntry.CreateTarEntry("Dockerfile");
                            byte[] fileBytes = Encoding.UTF8.GetBytes(build.Content);
                            tarEntry.Size = fileBytes.Length;
                            tarArchive.PutNextEntry(tarEntry);
                            tarArchive.Write(fileBytes, 0, fileBytes.Length);
                            tarArchive.CloseEntry();
                        }
                        tarball.Close();
                        tarball = new FileStream(TarFile, FileMode.Open);

                        await client.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
                        {
                            Tags = build.Labels.Values.ToList()
                        }, tarball, null, null, new Progress<JSONMessage>());
                    }
                    catch
                    {
                        try
                        {
                            File.Delete(TarFile);
                        }
                        catch { }
                        throw;
                    }

                    try
                    {
                        File.Delete(TarFile);
                    }
                    catch { }
                }
                break;
            case CreateImageType.Upload:
                {

                }
                break;
            case CreateImageType.Remote:
                {
                    await client.Images.BuildImageFromDockerfileAsync(new ImageBuildParameters
                    {
                        Dockerfile = build.DockerfileName,
                        Tags = build.Labels.Values.ToList(),
                        RemoteContext = build.RemoteUrl,
                    }, null, null, null, new Progress<JSONMessage>());
                }
                break;
        }

    }

    public static async Task<object?> ControlImageAsync(DockerClient client, DockerEvent @event, string id)
    {
        switch (@event.ImageType)
        {
            case ControlImageType.Export:

                break;
            case ControlImageType.Remove:
            case ControlImageType.ForceRemove:
                {
                    await client.Images.DeleteImageAsync(id, new ImageDeleteParameters
                    {
                        Force = @event.ImageType == ControlImageType.ForceRemove
                    });

                }
                break;
            case ControlImageType.View:
                {
                    var Image = await client.Images.InspectImageAsync(id);

                    return DockerImageInfo.Create(Image);
                }
                break;
            case ControlImageType.Layers:
                return await client.Images.GetImageHistoryAsync(id);
        }

        return null;
    }
}
