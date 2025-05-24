namespace DevSpaceShared.Events.Docker;
public class ListContainersEvent
{
    public Dictionary<string, string>? Filters { get; set; }
}
