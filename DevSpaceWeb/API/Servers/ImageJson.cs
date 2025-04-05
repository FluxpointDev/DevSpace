using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class ImageJson
{
    public ImageJson(DockerImageInfo image)
    {
        id = image.ID;
        name = image.Name;
        version = image.Version;
        created_at = image.Created;
        is_local = image.IsLocal;
        is_parent = image.IsParent;
        parent = image.Parent;
        size = image.Size;
        website = image.Website;
        source = image.Source;
        repo_tags = image.RepoTags;
        repo_digests = image.RepoDigests;
        docker_version = image.DockerVersion;
        operating_system = image.Os;
        operating_system_version = image.OsVersion;
        architecture = image.Architecture;
        comment = image.Comment;
        driver = image.Driver;
    }

    public string id { get; set; }
    public string name { get; set; }

    public string? version { get; set; }

    public DateTime created_at { get; set; }

    public bool is_local { get; set; }

    public bool is_parent { get; set; }

    public string? parent { get; set; }

    public long size { get; set; }

    public string? website { get; set; }

    public string? source { get; set; }

    public IList<string>? repo_tags { get; set; }

    public IList<string>? repo_digests { get; set; }

    public string? docker_version { get; set; }

    public string? operating_system { get; set; }

    public string? operating_system_version { get; set; }

    public string? architecture { get; set; }

    public string? comment { get; set; }

    public string? driver { get; set; }
}
