using DaRT;

namespace DevSpaceWeb.API.Consoles;

public class ConsolePlayerJson
{
    public ConsolePlayerJson(Player player, bool showIp)
    {
        number = player.number;
        if (showIp)
            ip = player.ip;
        ping = player.ping;
        guid = player.guid;
        name = player.name;
        status = player.status;
        location = player.location;
        comment = player.comment;
    }

    public int number { get; set; }
    public string? ip { get; set; }
    public string ping { get; set; }
    public string guid { get; set; }
    public string name { get; set; }
    public string status { get; set; }
    public string location { get; set; }
    public string comment { get; set; }
}
