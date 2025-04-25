using DevSpaceWeb.Data.Servers;

namespace DevSpaceWeb.API.Servers;

public class ServerJson
{
    public ServerJson(ServerData data, bool showIp)
    {
        id = data.Id.ToString();
        name = data.Name;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        created_at = data.CreatedAt;
        is_online = data.IsConnected;
        if (showIp)
            ip = data.AgentIp;
    }

    public string id;
    public string name;
    public string owner_id;
    public string? vanity_url;
    public DateTime created_at;
    public bool is_online;
    public string? ip;
}
