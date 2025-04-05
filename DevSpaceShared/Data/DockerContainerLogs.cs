namespace DevSpaceShared.Data;
public class DockerContainerLogs
{
    public string? ContainerName { get; set; }
    public DateTime? SinceDate { get; set; }
    public string? Logs { get; set; }
}
