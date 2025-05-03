namespace DaRT;

public class Kick
{
    public int id;
    public string name;
    public string? reason;

    public Kick(int id, string name, string? reason)
    {
        this.id = id;
        this.name = name;
        this.reason = reason;
    }
}