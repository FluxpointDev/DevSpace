using DevSpaceShared.Data;
using Docker.DotNet.Models;

namespace DevSpaceWeb.API.Servers;

public class ContainerJson
{
    public ContainerJson(DockerContainerInfo info)
    {
        id = info.Id;
        name = info.Name;
        image_id = info.ImageId;
        image_name = info.ImageName;
        stack_id = info.StackId;
        stack_name = info.StackName;
        state = info.State;
        status = info.Status;
        created_at = info.CreatedAt;
        is_running = info.IsRunning;
        is_paused = info.IsPaused;
        is_restarting = info.IsRestarting;
    }

    public ContainerJson(ContainerInspectResponse info)
    {
        id = info.ID;
        name = info.Name.Substring(1);
        image_id = info.Image;
        image_name = info.Config.Image;
        state = info.State.Status;
        created_at = info.Created;
        is_running = info.State.Running;
        is_paused = info.State.Paused;
        is_restarting = info.State.Restarting;
        if (is_running)
            status = "Running";
        else if (is_paused)
            status = "Paused";
        else if (is_restarting)
            status = "Restarting";
        else
        {
            status = "Exit " + info.State.ExitCode;
        }
    }

    public string id { get; set; }
    public string name { get; set; }
    public string image_id { get; set; }
    public string image_name { get; set; }
    public string? stack_id { get; set; }
    public string? stack_name { get; set; }
    public string status { get; set; }
    public string state { get; set; }
    public DateTime created_at { get; set; }
    public bool is_running { get; set; }
    public bool is_paused { get; set; }
    public bool is_restarting { get; set; }
}
