namespace DaRT;

public class BanIP
{
    public int id = 0;
    public String name = "";
    public String duration = "";
    public String reason = "";

    public BanIP(int id, String name, String duration, String reason)
    {
        this.id = id;
        this.name = name;
        this.duration = duration;
        this.reason = reason;
    }
}