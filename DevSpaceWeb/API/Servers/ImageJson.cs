using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class ImageJson
{
    public ImageJson(DockerImageInfo image)
    {
        id = image.ID;
        name = image.Name;
        version = image.Version;
        alt_version = image.AltVersion;
        created_at = image.Created;
        is_local = image.IsLocal;
        is_parent = image.IsParent;
        parent = image.Parent;
        size = image.Size;
        website = image.Website;
        docs = image.Docs;
        source = image.Source;
        repo_tags = image.RepoTags;
        repo_digests = image.RepoDigests;
        docker_version = image.DockerVersion;
        operating_system = image.Os;
        operating_system_version = image.OsVersion;
        architecture = image.Architecture;
        comment = image.Comment;
        driver = image.Driver;

        if (image.Config != null && image.Config.Environment != null)
            config.environment = image.Config.Environment;
        else
            config.environment = new List<string>();

        if (image.Config != null && image.Config.Labels != null)
            config.labels = image.Config.Labels;
        else
            config.labels = new Dictionary<string, string>();

        if (image.Config != null && image.Config.ExposedPorts != null)
            config.ports = image.Config.ExposedPorts;
        else
            config.ports = new List<string>();

        if (image.Config != null)
        {
            config.command = image.Config.Command;
            config.entrypoint = image.Config.Entrypoint;
            config.working_directory = image.Config.WorkingDirectory;
        }

    }

    public string id { get; set; }
    public string name { get; set; }
    public string? version { get; set; }
    public string? alt_version { get; set; }
    public DateTime created_at { get; set; }
    public bool is_local { get; set; }
    public bool is_parent { get; set; }
    public string? parent { get; set; }
    public long size { get; set; }
    public string? website { get; set; }
    public string? docs { get; set; }
    public string? source { get; set; }
    public IList<string>? repo_tags { get; set; }
    public IList<string>? repo_digests { get; set; }
    public string? docker_version { get; set; }
    public string? operating_system { get; set; }
    public string? operating_system_version { get; set; }
    public string? architecture { get; set; }
    public string? comment { get; set; }
    public string? driver { get; set; }
    public ImageConfigJson config { get; set; } = new ImageConfigJson();
}
public class ImageConfigJson
{
    public IList<string> environment { get; set; }
    public IDictionary<string, string>? labels { get; set; }
    public string? command { get; set; }
    public string? entrypoint { get; set; }
    public IList<string>? ports { get; set; }
    public string? working_directory { get; set; }
}