using Docker.DotNet.Models;

namespace DevSpaceWeb.API.Servers;

public class ContainerChangeJson
{
    public ContainerChangeJson(ContainerFileSystemChangeResponse json)
    {
        type = json.Kind;
        path = json.Path;
    }
    public FileSystemChangeKind type { get; set; }
    public string? path { get; set; }
}
