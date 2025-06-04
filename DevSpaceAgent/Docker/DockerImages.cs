using CliWrap;
using DevSpaceShared;
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
        List<DockerImageInfo> Response = [];
        HashSet<string> ParentMap = [];
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

        foreach (DockerImageInfo i in Response)
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

    public static async Task PruneImagesAsync(DockerClient client)
    {
        await Cli.Wrap("docker")
            .WithArguments(["image", "prune", "-a", "-f"])
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
    }

    public static async Task PullImageAsync(DockerClient client, string? name)
    {
        if (string.IsNullOrEmpty(name))
            throw new Exception("Image name is missing.");

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

    public static async Task CreateImageAsync(DockerClient client, CreateImageEvent? build)
    {
        if (build == null)
            throw new Exception("Invalid image creation options.");

        switch (build.Type)
        {
            case CreateImageType.Editor:
                {
                    string TarFile = Program.CurrentDirectory + "Data/Temp/" + Guid.NewGuid().ToString() + ".tar";

                    try
                    {
                        FileStream tarball = new FileStream(TarFile, FileMode.Create);
#pragma warning disable CS0618 // Type or member is obsolete
                        using (TarOutputStream tarArchive = new TarOutputStream(tarball))
                        {
                            TarEntry tarEntry = TarEntry.CreateTarEntry("Dockerfile");
                            byte[] fileBytes = Encoding.UTF8.GetBytes(build.Content);
                            tarEntry.Size = fileBytes.Length;
                            tarArchive.PutNextEntry(tarEntry);
                            tarArchive.Write(fileBytes, 0, fileBytes.Length);
                            tarArchive.CloseEntry();
                        }
#pragma warning restore CS0618 // Type or member is obsolete
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
                    if (@event.ImageType == ControlImageType.ForceRemove)
                    {
                        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                        {
                            All = true,
                            Filters = new Dictionary<string, IDictionary<string, bool>>
                            {
                                { "ancestor", new Dictionary<string, bool>
                                { { id, true }}
                                }
                            }
                        });
                        foreach (ContainerListResponse? i in Containers)
                        {
                            if (i.IsRunning())
                            {
                                await client.Containers.StopContainerAsync(i.ID, new ContainerStopParameters());
                            }
                        }
                    }

                    await client.Images.DeleteImageAsync(id, new ImageDeleteParameters
                    {
                        Force = @event.ImageType == ControlImageType.ForceRemove
                    });
                }
                break;
            case ControlImageType.View:
                {
                    ImageInspectResponse Image = await client.Images.InspectImageAsync(id);

                    DockerImageInfo Info = DockerImageInfo.Create(Image);

                    try
                    {
                        IList<ContainerListResponse> Containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                        {
                            All = true,
                            Filters = new Dictionary<string, IDictionary<string, bool>>
                            {
                                { "ancestor", new Dictionary<string, bool>
                                { { id, true }}
                                }
                            }
                        });
                        Info.ContainersCount = Containers.Count;
                    }
                    catch
                    {
                        Info.ContainersCount = -1;
                    }

                    return Info;
                }
            case ControlImageType.Layers:
                return await client.Images.GetImageHistoryAsync(id);
        }

        return null;
    }
}
