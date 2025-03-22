namespace DevSpaceShared.Events.Docker;
public class ContainerLogsEvent
{
    public int Limit { get; set; }
    public bool ShowTimestamp { get; set; }
    public DateTime? SinceDate { get; set; }
    public DateTime? UntilDate { get; set; }
}
