using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.WebSocket;
using MongoDB.Driver;
using NetCoreServer;
using System.Security.Authentication;

namespace DevSpaceWeb.Data.Servers;

public class ServerData : ITeamResource
{
    public string AgentIp { get; set; }
    public string AgentKey { get; set; }
    public short AgentPort { get; internal set; }

    private ServerWebSocket WebSocket;
    public ServerWebSocket GetWebSocket(bool create = true)
    {
        if (WebSocket == null && create)
        {
            WebSocket = new ServerWebSocket();
            if (!Program.IsPreviewMode)
                WebSocket.Run(this);
        }
        return WebSocket;
    }
    public void SetWebSocket(ServerWebSocket websocket)
    {
        WebSocket = websocket;
    }

    public bool IsRunningDockerContainer()
    {
        if (WebSocket != null && WebSocket.Client != null && WebSocket.Client.IsRunningDockerContainer)
            return true;
        return false;
    }

    public async Task UpdateAsync(UpdateDefinition<ServerData> update, Action action)
    {
        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Servers.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public async Task DeleteAsync(TeamMemberData member, Action action)
    {
        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Servers.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.ServerDeleted)
                .SetTarget(Team)
                .AddProperty("Name", Name));

            _DB.Servers.Cache.TryRemove(Id, out _);

            if (GetWebSocket() != null)
                GetWebSocket().Client.Dispose();

            action?.Invoke();
        }
    }
}
public class ServerWebSocket
{
    public WebSocketClient Client;
    public void Run(ServerData server)
    {
        ValidateCert ValidateCert = new ValidateCert();
        SslContext context = new SslContext(SslProtocols.Tls12, (e, b, l, m) =>
        {
            if (b != null)
            {
                ValidateCert.Cert = b.GetCertHashString();
                if (b.Subject == "CN=devspace")
                {
                    Logger.LogMessage($"Cert: {b.Subject} " + ValidateCert.Cert, LogSeverity.Debug);
                    return true;
                }
            }

            if (m == System.Net.Security.SslPolicyErrors.None)
                return false;
            return false;

        });
        context.ClientCertificateRequired = false;

        Logger.LogMessage($"Connecting to {server.AgentIp}:{server.AgentPort}", LogSeverity.Info);
        Client = new WebSocketClient(context, server.AgentIp, server.AgentPort)
        {
            ValidateCert = ValidateCert,
            Key = server.AgentKey
        };
        // Connect the client
        Logger.LogMessage("Client connecting...", LogSeverity.Debug);
        bool Connected = Client.ConnectAsync();
        Logger.LogMessage("Is Connected: " + Connected, LogSeverity.Debug);
    }
}
