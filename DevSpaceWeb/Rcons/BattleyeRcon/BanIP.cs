namespace DaRT;

public class BanIP
{
    public int id = 0;
    public string name = "";
    public string duration = "";
    public string? reason = "";

    public BanIP(int id, string name, string duration, string? reason)
    {
        this.id = id;
        this.name = name;
        this.duration = duration;
        this.reason = reason;
    }
}