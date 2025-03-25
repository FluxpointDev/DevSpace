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
        status = info.State;
        created_at = info.CreatedAt;
        is_running = info.IsRunning;
        is_paused = info.IsPaused;
        is_restarting = info.IsRestarting;
    }

    public ContainerJson(ContainerInspectResponse info)
    {
        id = info.ID;
        name = info.Name;
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

    public string id;
    public string name;
    public string image_id;
    public string image_name;
    public string stack_id;
    public string stack_name;
    public string status;
    public string state;
    public DateTime created_at;
    public bool is_running;
    public bool is_paused;
    public bool is_restarting;
}
