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
            WebSocket.Run(this);
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
    public void Run(ServerData server)
    {
        ValidateCert ValidateCert = new ValidateCert();
        var context = new SslContext(SslProtocols.Tls12, (e, b, l, m) =>
        {
            if (b != null)
            {
                ValidateCert.Cert = b.GetCertHashString();
                if (b.Subject == "CN=devspace")
                {
                    Console.WriteLine($"Cert: {b.Subject} " + ValidateCert.Cert);
                    return true;
                }
            }

            if (m == System.Net.Security.SslPolicyErrors.None)
                return false;
            return false;

        });
        context.ClientCertificateRequired = false;

        Console.WriteLine("Connecting to 127.0.0.1:" + server.AgentPort);
        Client = new WebSocketClient(context, "127.0.0.1", server.AgentPort)
        {
            ValidateCert = ValidateCert,
            Key = server.AgentKey
        };
        // Connect the client
        Console.WriteLine("Client connecting...");
        bool Connected = Client.ConnectAsync();
        Console.WriteLine("Is Connected: " + Connected);
    }
}
