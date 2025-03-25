using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class StackJson
{
    public StackJson(DockerStackInfo info)
    {
        id = info.Id;
        name = info.Name;
        created_at = info.CreatedAt;
        updated_at = info.UpdatedAt;
        version = info.Version;
        type = info.Type;
        control_type = info.ControlType;
        containers_count = info.ContainersCount;
        is_running = info.IsRunning;
        containers = info.Containers.Select(x => new ContainerJson(x)).ToArray();
    }

    public string id;
    public string name;
    public DateTime? created_at;
    public DateTime? updated_at;
    public long version;
    public DockerStackType type;
    public DockerStackControl control_type;
    public int containers_count;
    public bool is_running;
    public ContainerJson[] containers;
}