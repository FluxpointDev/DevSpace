using DevSpaceWeb.Data.Servers;

namespace DevSpaceWeb.API.Servers;

public class ServerJson
{
    public ServerJson(ServerData data)
    {
        id = data.Id.ToString();
        team_id = data.TeamId.ToString();
        name = data.Name;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        created_at = data.CreatedAt;
        var WebSocket = data.GetWebSocket(false);
        if (WebSocket != null && WebSocket.Client.IsConnected)
            is_online = true;
    }

    public string id { get; set; }
    public string team_id { get; set; }
    public string name { get; set; }
    public string owner_id { get; set; }
    public string? vanity_url { get; set; }
    public DateTime created_at { get; set; }
    public bool is_online { get; set; }
}
