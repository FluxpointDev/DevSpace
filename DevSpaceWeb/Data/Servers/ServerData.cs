using DevSpaceShared.Responses;
using DevSpaceShared.Services;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Agents;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Servers;

public class ServerData : ITeamResource
{
    public ServerData() : base(ResourceType.Server) { }

    public string? AgentId { get; set; }
    public required string AgentIp { get; set; }
    public required string AgentKey { get; set; }
    public short AgentPort { get; set; }
    public ServerAgentType AgentType { get; set; }

    private IAgent? WebSocket;

    [BsonIgnore]
    [JsonIgnore]
    public bool IsConnected => WebSocket != null && WebSocket.IsConnected;

    [BsonIgnore]
    [JsonIgnore]
    public WebSocketErrorType? WebSocketError => WebSocket != null ? WebSocket.Error : WebSocketErrorType.NoConnection;

    public async Task StartWebSocket(bool reconnect = false)
    {
        switch (AgentType)
        {
            case ServerAgentType.Client:
                {
                    WebSocket = new ClientAgent
                    {
                        Server = this
                    };
                }
                break;
            case ServerAgentType.Edge:
                {
                    WebSocket = new EdgeAgent
                    {
                        Server = this
                    };
                }
                break;
        }
        await WebSocket.Connect(AgentIp, AgentPort, AgentKey, reconnect);
    }

    public void SetWebSocket(IAgent socket)
    {
        WebSocket = socket;
    }

    public bool HasVersionOrHigher(Version version)
    {
        Version? ver = null;

        IAgent? WebSocket = GetWebSocket();
        if (WebSocket == null)
            return false;

        if (WebSocket.Stats != null && !string.IsNullOrEmpty(WebSocket.Stats.AgentVersion))
            Version.TryParse(WebSocket.Stats.AgentVersion, out ver);

        if (ver == null && WebSocket is ClientAgent CA && CA.Discover != null && !string.IsNullOrEmpty(CA.Discover.Version))
            Version.TryParse(CA.Discover.Version, out ver);

        if (ver == null)
            return false;

        return ver >= version;
    }

    public string GetAgentVersion()
    {
        IAgent? WebSocket = GetWebSocket();
        if (WebSocket == null)
            return "Unknown";

        if (WebSocket.Stats != null && !string.IsNullOrEmpty(WebSocket.Stats.AgentVersion))
            return "v" + WebSocket.Stats.AgentVersion;

        if (WebSocket is ClientAgent CA && CA.Discover != null && !string.IsNullOrEmpty(CA.Discover.Version))
            return "v" + CA.Discover.Version;

        return "Unknown";
    }

    public void RemoveWebSocket()
    {
        if (WebSocket != null)
        {
            if (WebSocket is ClientAgent CA)
            {
                CA.StopReconnect = true;
                CA.WebSocket.DisconnectAndStop();
                CA.WebSocket.Dispose();
            }

            WebSocket = null;
        }
    }

    public IAgent? GetWebSocket()
    {
        return WebSocket;
    }

    public bool TryGetWebSocket(out IAgent? agent)
    {
        if (WebSocket != null)
        {
            agent = WebSocket!;
            return true;
        }

        agent = null;
        return false;
    }

    public async Task<SocketResponse<T?>> RunJsonAsync<T>(Radzen.NotificationService notification, IWebSocketTask json, Action<SocketResponse<T?>>? action = null, CancellationToken token = default) where T : class
    {
        if (WebSocket == null)
            await StartWebSocket();

        if (WebSocket == null || WebSocket.Error.HasValue)
            return new SocketResponse<T?> { Error = ClientError.Exception };

        if (WebSocket is ClientAgent CA && CA.WebSocket != null)
            return await CA.RunJsonAsync(notification, json, action, token);
        else if (WebSocket is EdgeAgent EA && EA.WebSocket != null)
            return await EA.RunJsonAsync(notification, json, action, token);

        return new SocketResponse<T?> { Error = ClientError.Exception };
    }

    public async Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
    {
        if (WebSocket == null)
            await StartWebSocket();

        if (WebSocket == null || WebSocket.Error.HasValue)
            return new SocketResponse<T?> { Error = ClientError.Exception };

        return await WebSocket.RecieveJsonAsync<T>(json, token);
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
                .SetTarget(this));

            _DB.Servers.Cache.TryRemove(Id, out _);

            RemoveWebSocket();

            action?.Invoke();
        }
    }
}
public enum ServerAgentType
{
    Client, Edge
}