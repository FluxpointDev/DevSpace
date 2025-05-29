namespace DevSpaceShared.Events.Docker;

public class ContainerRecreateEvent
{
    public bool RepullImage { get; set; }
    public string NewName { get; set; }
    public string NewImage { get; set; }
}
