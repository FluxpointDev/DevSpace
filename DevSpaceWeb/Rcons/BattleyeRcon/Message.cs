namespace DaRT;

public class Message
{
    public int id = 0;
    public String? name;
    public String? message;

    public Message(int id, String? name, String? message)
    {
        this.id = id;
        this.name = name;
        this.message = message;
    }
}
