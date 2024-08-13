using DevSpaceWeb.Database;
using DevSpaceWeb.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NetCoreServer;
using System.Security.Authentication;

namespace DevSpaceWeb.Data;

public class ServerData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId TeamId { get; set; }
    public string AgentIp { get; set; }
    public string AgentKey { get; set; }
    public short AgentPort { get; internal set; }

    private ServerWebSocket WebSocket;
    public ServerWebSocket GetWebSocket()
    {
        if (WebSocket == null)
        {
            WebSocket = new ServerWebSocket();
            WebSocket.Run();
        }
        return WebSocket;
    }

    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }
}
public class ServerWebSocket
{
    public WebSocketClient Client;
    public void Run()
    {
        return;
        ValidateCert ValidateCert = new ValidateCert();
        var context = new SslContext(SslProtocols.Tls12, (e, b, l, m) =>
        {
            if (b != null)
            {
                ValidateCert.Cert = b.GetCertHashString();
                Console.WriteLine($"Cert: {b.Subject} " + ValidateCert);
                if (b.Subject == "CN=devspace")
                    return true;
            }

            if (m == System.Net.Security.SslPolicyErrors.None)
                return true;
            return false;

        });
        context.ClientCertificateRequired = false;
        Client = new WebSocketClient(context,
            "127.0.0.1",
            5555)
        { ValidateCert = ValidateCert };
        // Connect the client
        Console.Write("Client connecting...");
        Client.ConnectAsync();
    }
}
