using Docker.DotNet.Models;

namespace DevSpaceShared.Data;

public class DockerContainerInfo
{
    public static DockerContainerInfo Create(ContainerListResponse data)
    {
        DockerContainerInfo Info = new DockerContainerInfo
        {
            Id = data.ID,
            Name = data.Names != null ? data.Names.First().Substring(1) : data.ID,
            ImageId = data.ImageID,
            ImageName = data.Image,
            State = data.State,
            Status = data.Status,
            CreatedAt = data.Created
        };

        if (data.Labels != null)
        {
            string? Id = null;
            if (data.Labels.TryGetValue("com.docker.compose.project", out string? label) && !string.IsNullOrEmpty(label))
            {
                Info.StackName = label;
                Id = label;
            }

            string? DataPath = null;

            if (data.Labels.TryGetValue("com.docker.compose.project.config_files", out string? configFile))
                DataPath = configFile;

            if (data.Labels.TryGetValue("com.docker.compose.project.working_dir", out string? workDir))
                DataPath = workDir;

            if (!string.IsNullOrEmpty(DataPath))
            {
                if (DataPath.StartsWith("/data/compose"))
                {
                    try
                    {
                        Id = DataPath.Split('/', StringSplitOptions.RemoveEmptyEntries)[2];
                    }
                    catch { }
                }
                else if (DataPath.StartsWith("/app/Data/Stacks/"))
                    Id = DataPath.Split('/')[4];
            }
            Info.StackId = Id;
        }

        return Info;
    }

    public required string Id { get; set; }
    public string Name { get; set; }
    public string ImageId { get; set; }
    public string ImageName { get; set; }
    public string? StackId { get; set; }
    public string? StackName { get; set; }
    public string State { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRunning => State != null && State.StartsWith("running");
    public bool IsRestarting => State != null && State == "restarting";
    public bool IsPaused => State != null && State == "paused";

    public bool IsOnline()
    {
        if (State == null)
            return false;

        if (State.StartsWith("running") || State == "restarting")
            return true;

        return false;
    }
}
