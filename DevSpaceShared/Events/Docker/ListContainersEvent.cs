namespace DevSpaceShared.Events.Docker;

public class ListContainersEvent
{
    public IDictionary<string, string>? Filters { get; set; }
}
