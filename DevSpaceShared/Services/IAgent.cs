using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace DevSpaceShared.Services;

public class IAgent
{
    public AgentStatsResponse? Stats { get; set; }

    public WebSocketErrorType? Error;

    [JsonIgnore]
    public ConcurrentDictionary<string, TaskCompletionSource<JToken>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JToken>>();

    public virtual bool IsConnected { get; set; }

    public virtual async Task Connect(string host, short port, string key, bool reconnect = false)
    {

    }

    public virtual async Task Disconnect()
    {

    }

    public virtual async Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
    {
        return new SocketResponse<T?> { Error = ClientError.Exception };
    }
}
public enum WebSocketErrorType
{
    NoConnection, AgentUnsupported, AuthFailed, AgentError, ServerError
}