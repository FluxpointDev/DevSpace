namespace DaRT;

public class BanOffline
{
    public string guid = "";
    public string name = "";
    public string duration = "";
    public string? reason = "";

    public BanOffline(string guid, string name, string duration, string? reason)
    {
        this.guid = guid;
        this.name = name;
        this.duration = duration;
        this.reason = reason;
    }
}
