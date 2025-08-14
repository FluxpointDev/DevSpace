using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Consoles;
using LibMCRcon;

namespace DevSpaceWeb.API.Consoles;

public class ConsoleJson
{
    public ConsoleJson(ConsoleData data, bool showIp)
    {
        if (showIp)
            ip = data.Ip;
        port = data.Port;
        type = data.Type;
        switch (data.Type)
        {
            case ConsoleType.Battleye:
                {
                    if (_Data.BattleyeRcons.TryGetValue(data.Id, out DaRT.RCon? rcon) && rcon.IsConnected)
                        is_online = true;
                }
                break;
            case ConsoleType.Minecraft:
                {
                    if (_Data.MinecraftRcons.TryGetValue(data.Id, out MCRconAsync? rcon) && rcon.IsConnected)
                        is_online = true;
                }
                break;
        }
        created_at = data.CreatedAt;
    }

    public int port;
    public ConsoleType type;
    public bool is_online;
    public DateTime created_at;
    public string? ip;
}
