namespace DevSpaceShared.Data;
public class DockerEventInfo
{
    public string Status { get; set; }
    public string Id { get; set; }
    public string From { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public DockerEventInfoActor Actor { get; set; }
    public string Scope { get; set; }
    public long Time { get; set; }
}
public class DockerEventInfoActor
{
    public string Id { get; set; }
    public Dictionary<string, object> Attributes { get; set; }
}