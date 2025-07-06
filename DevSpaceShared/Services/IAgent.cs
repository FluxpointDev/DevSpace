using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DevSpaceShared.Services;

public class IAgent
{
    public AgentStatsResponse? Stats { get; set; }

    public WebSocketErrorType? Error;

    [JsonIgnore]
    public ConcurrentDictionary<string, TaskCompletionSource<JsonElement>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JsonElement>>();

    public virtual bool IsConnected { get; set; }

    public virtual async Task Connect(string host, short port, string key, bool reconnect = false)
    {

    }

    public virtual async Task Disconnect()
    {

    }

    public virtual async Task<SocketResponse<T?>> RecieveJsonAsync<T, InputJson>(InputJson json, CancellationToken token = default) where T : class where InputJson : IWebSocketTask
    {
        return new SocketResponse<T?> { Error = ClientError.Exception };
    }
}
public enum WebSocketErrorType
{
    NoConnection, AgentUnsupported, AuthFailed, AgentError, ServerError
}