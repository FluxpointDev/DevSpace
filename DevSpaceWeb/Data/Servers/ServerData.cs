using DevSpaceShared.Data;
using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NetCoreServer;
using Newtonsoft.Json;
using System.Net;
using System.Security.Authentication;

namespace DevSpaceWeb.Data.Servers;

public class ServerData : ITeamResource
{
    public ServerData() : base(ResourceType.Server) { }

    public string AgentId { get; set; }
    public string AgentIp { get; set; }
    public string AgentKey { get; set; }
    public short AgentPort { get; internal set; }

    private ServerWebSocket WebSocket;

    [BsonIgnore]
    [JsonIgnore]
    public bool IsConnected => WebSocket != null && WebSocket.Client != null && WebSocket.Client.IsConnected;

    [BsonIgnore]
    [JsonIgnore]
    public ServerWebSocketErrorType? WebSocketError => WebSocket != null ? WebSocket.Error : ServerWebSocketErrorType.NoConnection;

    public async Task StartWebSocket(bool reconnect = false)
    {
        WebSocket = new ServerWebSocket();
        await WebSocket.RunAsync(this, reconnect);
    }

    public void SetWebSocket(ServerWebSocket socket)
    {
        WebSocket = socket;
    }

    public void RemoveWebSocket()
    {
        if (WebSocket != null)
        {
            WebSocket.StopReconnect = true;
            if (WebSocket.Client != null)
            {
                WebSocket.Client.DisconnectAndStop();
                WebSocket.Client.Dispose();
            }

            WebSocket = null;
        }
    }

    public ServerWebSocket? GetWebSocket()
    {
        return WebSocket;
    }

    public async Task<SocketResponse<T?>> RunJsonAsync<T>(Radzen.NotificationService notification, IWebSocketTask json, Action<SocketResponse<T?>>? action = null, CancellationToken token = default) where T : class
    {
        if (WebSocket == null)
            await StartWebSocket();

        if (WebSocket.Error.HasValue || WebSocket.Client == null)
            return new SocketResponse<T?> { Error = ClientError.Exception };

        return await WebSocket.Client.RunJsonAsync(notification, json, action, token);
    }

    public async Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
    {
        if (WebSocket == null)
            await StartWebSocket();

        if (WebSocket.Error.HasValue || WebSocket.Client == null)
            return new SocketResponse<T?> { Error = ClientError.Exception };
        return await WebSocket.Client.RecieveJsonAsync<T>(json, token);
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

            RemoveWebSocket();

            action?.Invoke();
        }
    }
}
public class ServerWebSocket
{
    public WebSocketClient? Client;
    public ServerWebSocketErrorType? Error;
    public DiscoverAgentInfo? Discover;
    public bool StopReconnect;
    public async Task DiscoverAsync(ServerData server)
    {
        Console.WriteLine("Discover");
        HttpRequestMessage Req = new HttpRequestMessage(HttpMethod.Get, "https://" + server.AgentIp + ":" + server.AgentPort + "/discover");
        Req.Headers.Add("Connection", "Upgrade");
        HttpResponseMessage? Response = null;
        try
        {
            Response = await Program.AgentDiscoverHttp.SendAsync(Req);
        }
        catch (HttpRequestException he)
        {
            Console.WriteLine("Status: " + he.HttpRequestError.ToString());
            switch (he.HttpRequestError)
            {
                case HttpRequestError.ConnectionError:
                case HttpRequestError.NameResolutionError:
                case HttpRequestError.SecureConnectionError:
                    {
                        Error = ServerWebSocketErrorType.NoConnection;
                    }
                    return;
            }
        }
        catch { }
        if (Response == null)
        {
            Error = ServerWebSocketErrorType.ServerError;
            return;
        }
        Console.WriteLine("Error: " + Response.StatusCode.ToString());
        if (!Response.IsSuccessStatusCode)
        {
            switch (Response.StatusCode)
            {
                case System.Net.HttpStatusCode.BadGateway:
                case System.Net.HttpStatusCode.RequestTimeout:
                case System.Net.HttpStatusCode.GatewayTimeout:
                    Error = ServerWebSocketErrorType.NoConnection;
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    Error = ServerWebSocketErrorType.AgentUnsupported;
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    Error = ServerWebSocketErrorType.AuthFailed;
                    break;
                default:
                    Error = ServerWebSocketErrorType.AgentError;
                    break;
            }

            return;
        }
        try
        {
            using (var responseContent = await Response.Content.ReadAsStreamAsync())
                Discover = await System.Text.Json.JsonSerializer.DeserializeAsync<DiscoverAgentInfo>(responseContent);
        }
        catch
        {
            Error = ServerWebSocketErrorType.ServerError;
        }

        if (Discover == null)
            Error = ServerWebSocketErrorType.ServerError;
    }

    public async Task RunAsync(ServerData server, bool reconnect = false)
    {
        await DiscoverAsync(server);

        if (Error.HasValue && Error != ServerWebSocketErrorType.NoConnection)
            return;

        if (Discover == null)
        {
            if (!StopReconnect && reconnect)
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
                await RunAsync(server, true);
            }
            return;
        }

        if (Discover.Id != server.AgentId)
        {
            Error = ServerWebSocketErrorType.AgentError;

            return;
        }
        Error = null;

        SslContext context = new SslContext(SslProtocols.None, (e, b, l, m) =>
        {
            if (b != null)
            {
                if (b.Subject == "CN=devspace")
                {
                    return true;
                }
            }
            return false;

        });

        IPAddress? address = null;
        if (!IPAddress.TryParse(server.AgentIp, out address))
        {
            IPHostEntry Host = Dns.GetHostEntry(server.AgentIp);
            address = Host.AddressList.FirstOrDefault();
        }

        if (address == null)
        {
            Error = ServerWebSocketErrorType.NoConnection;
            if (!StopReconnect && reconnect)
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
                await RunAsync(server, true);
            }
            return;
        }

        Logger.LogMessage($"Connecting to {server.AgentIp}:{server.AgentPort}", LogSeverity.Info);
        Client = new WebSocketClient(context, address, server.AgentPort)
        {
            Key = server.AgentKey
        };

        Logger.LogMessage("Client connecting...", LogSeverity.Debug);
        bool Connected = Client.ConnectAsync();
        if (!Connected)
            Error = ServerWebSocketErrorType.NoConnection;
    }
}
public enum ServerWebSocketErrorType
{
    NoConnection, AgentUnsupported, AuthFailed, AgentError, ServerError
}
