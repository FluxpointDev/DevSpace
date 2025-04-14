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

    public ContainerJson(ContainerInspectResponse info, bool viewDetails, bool viewEnvironment, bool viewHealthLog)
    {
        id = info.ID;
        name = info.Name.Substring(1);
        if (info.Config.Labels != null && info.Config.Labels.TryGetValue("org.opencontainers.image.description", out string? desc))
            description = desc;
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
        if (viewDetails)
        {
            details = new ContainerDetailsJson(info);
            labels = info.Config.Labels;
            if (labels == null)
                labels = new Dictionary<string, string>();
        }
        if (viewEnvironment)
        {
            if (info.Config.Env == null)
                environment = new Dictionary<string, string>();
            else
                environment = info.Config.Env.ParseEnvironment().ToDictionary(x => x.Key, x => x.Value);
        }

        health = new ContainerHealthJson(info, viewHealthLog);
    }

    public string id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public string image_id { get; set; }
    public string? image_name { get; set; }
    public string? stack_id { get; set; }
    public string? stack_name { get; set; }
    public string status { get; set; }
    public string state { get; set; }
    public DateTime created_at { get; set; }
    public bool is_running { get; set; }
    public bool is_paused { get; set; }
    public bool is_restarting { get; set; }
    public ContainerDetailsJson? details { get; set; }
    public IDictionary<string, string>? labels { get; set; }
    public IDictionary<string, string?>? environment { get; set; }
    public ContainerHealthJson? health { get; set; }
}
public class ContainerDetailsJson
{
    public ContainerDetailsJson(ContainerInspectResponse container)
    {
        if (container.Config.Cmd != null)
            command = string.Join(" ", container.Config.Cmd);

        if (container.Config.Entrypoint != null)
            entrypoint = string.Join(" ", container.Config.Entrypoint);

        working_directory = container.Config.WorkingDir;
        platform = Utils.FriendlyName(container.Platform);
        network = container.HostConfig.NetworkMode;
        isOOMDisabled = container.HostConfig.OomKillDisable.GetValueOrDefault();
        isPrivileged = container.HostConfig.Privileged;
        IsReadOnly = container.HostConfig.ReadonlyRootfs;
        NetworkDisabled = container.Config.NetworkDisabled;
    }

    public string? command { get; set; }
    public string? entrypoint { get; set; }
    public string working_directory { get; set; }
    public string platform { get; set; }
    public string network { get; set; }
    public bool isOOMDisabled { get; set; }
    public bool isPrivileged { get; set; }
    public bool IsReadOnly { get; set; }
    public bool NetworkDisabled { get; set; }
}
public class ContainerHealthJson
{
    public ContainerHealthJson(ContainerInspectResponse container, bool viewHealthLog)
    {
        if (!string.IsNullOrEmpty(container.State.StartedAt))
            started_at = new DateJson(container.State.StartedAt);

        if (!string.IsNullOrEmpty(container.State.FinishedAt) && container.State.StartedAt != container.State.FinishedAt)
            stopped_at = new DateJson(container.State.FinishedAt);

        error = container.GetExitMessage();
        if (container.State.Health == null || container.State.Health.Status == "none")
            check = "none";
        else
            check = container.State.Health.Status;

        if (container.State.Health != null)
        {
            failures = container.State.Health.FailingStreak;
            if (viewHealthLog && container.State.Health.Log != null && container.State.Health.Log.Any())
                log_json = container.State.Health.Log.First().Output;
        }
    }

    public DateJson? started_at { get; set; }
    public DateJson? stopped_at { get; set; }
    public string? error { get; set; }
    public string check { get; set; }
    public long failures { get; set; }
    public string? log_json { get; set; }
}
