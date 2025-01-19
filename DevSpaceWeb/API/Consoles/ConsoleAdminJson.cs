using DaRT;

namespace DevSpaceWeb.API.Consoles;

public class ConsoleAdminJson
{
    public ConsoleAdminJson(RconAdmin admin, bool showIp)
    {
        id = admin.Id;
        if (showIp)
            ip = admin.Ip;
    }

    public int id { get; set; }
    public string? ip { get; set; }
}
