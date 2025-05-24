namespace DevSpaceShared.Events.Docker;
public class CreateStackEvent
{
    public string? Name { get; set; }
    public string? Content { get; set; }
    public string? DockerfileName { get; set; }
    public bool IsAdministrator { get; set; }
    public bool PullImage { get; set; }
}
