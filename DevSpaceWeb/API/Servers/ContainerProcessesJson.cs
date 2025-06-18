using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class ContainerProcessesJson
{
    public ContainerProcessesJson(DockerContainerProcesses processes)
    {
        this.processes = processes.Data!.Processes.Select(x => new ContainerProcessJson(x)).ToList();
    }

    public List<ContainerProcessJson> processes { get; set; }
}
public class ContainerProcessJson
{
    public ContainerProcessJson(IList<string> list)
    {
        user = list[0];
        process_id = long.Parse(list[1]);
        parent_id = long.Parse(list[2]);
        threads = long.Parse(list[3]);
        total_cpu_usage = double.Parse(list[4]);
        current_cpu_usage = double.Parse(list[5]);
        current_memory_usage = double.Parse(list[6]);
        start_time = $"{list[7]} {list[8]} {list[9]} {list[10]} {list[11]}";
        elapsed_time = list[8];
        name = list[9];
        command = list[10];
    }

    public string user { get; set; }
    public long process_id { get; set; }
    public long parent_id { get; set; }
    public long threads { get; set; }
    public double total_cpu_usage { get; set; }
    public double current_cpu_usage { get; set; }
    public double current_memory_usage { get; set; }
    public string start_time { get; set; }
    public string elapsed_time { get; set; }
    public string name { get; set; }
    public string command { get; set; }
}
