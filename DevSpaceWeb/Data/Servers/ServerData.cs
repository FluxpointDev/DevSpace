﻿using DevSpaceWeb.Data.Teams;
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
    public ServerWebSocket GetWebSocket()
    {
        if (WebSocket == null)
        {
            WebSocket = new ServerWebSocket();
            if (!Program.IsPreviewMode)
                WebSocket.Run(this);
        }
        return WebSocket;
    }

    public bool IsRunningDockerContainer()
    {
        if (WebSocket != null && WebSocket.Client != null && WebSocket.Client.IsRunningDockerContainer)
            return true;
        return false;
    }

    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }

    public async Task UpdateAsync(UpdateDefinition<ServerData> update)
    {
        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Servers.Collection.UpdateOneAsync(filter, update);
        if (!Result.IsAcknowledged)
            throw new InvalidOperationException("Failed to update server data");
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
