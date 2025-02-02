using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Consoles;

namespace DevSpaceWeb.API.Consoles;

public class ConsoleJson
{
    public ConsoleJson(ConsoleData data)
    {
        ip = data.Ip;
        port = data.Port;
        type = data.Type;
        switch (data.Type)
        {
            case ConsoleType.Battleye:
                {
                    if (_Data.BattleyeRcons.TryGetValue(data.Id, out var rcon) && rcon.IsConnected)
                        is_online = true;
                }
                break;
            case ConsoleType.Minecraft:
                {
                    if (_Data.MinecraftRcons.TryGetValue(data.Id, out var rcon) && rcon.IsConnected)
                        is_online = true;
                }
                break;
        }
        created_at = data.CreatedAt;
    }

    public string ip { get; set; }
    public int port { get; set; }
    public ConsoleType type { get; set; }
    public bool is_online { get; set; }
    public DateTime created_at { get; set; }
}
