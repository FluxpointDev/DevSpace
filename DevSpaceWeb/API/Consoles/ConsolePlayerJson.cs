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

    public int number;
    public string? ip;
    public string? ping;
    public string? guid;
    public string? name;
    public string? status;
    public string? location;
    public string? comment;
}
