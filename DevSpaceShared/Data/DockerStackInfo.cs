
namespace DevSpaceShared.Data;

public class DockerStackInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Version { get; set; }
    public DockerStackType Type { get; set; }
    public List<string> Containers { get; set; } = new List<string>();
    public bool IsRunning;
}
public enum DockerStackType
{
    Compose, Portainer, Repo, System
}
