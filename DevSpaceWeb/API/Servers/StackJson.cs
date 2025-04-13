using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class StackJson
{
    public StackJson(DockerStackInfo info)
    {
        id = info.Id;
        name = info.Name;
        if (string.IsNullOrEmpty(id))
            id = name;
        created_at = info.CreatedAt;
        updated_at = info.UpdatedAt;
        version = info.Version;
        type = info.Type;
        control_type = info.ControlType;
        containers_count = info.ContainersCount;
        is_running = info.IsRunning;
        containers = info.Containers.Select(x => new ContainerJson(x)).ToArray();
    }

    public string id { get; set; }
    public string? name { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public long version { get; set; }
    public DockerStackType type { get; set; }
    public DockerStackControl control_type { get; set; }
    public int containers_count { get; set; }
    public bool is_running { get; set; }
    public ContainerJson[] containers { get; set; }
}