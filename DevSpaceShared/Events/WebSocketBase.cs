using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;

namespace DevSpaceShared.WebSocket;

public class WebSocketBase<T> where T : System.Net.WebSockets.WebSocket
{
    public string? Id;
    public T? Socket;
    public ConcurrentDictionary<string, TaskCompletionSource<JsonElement>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JsonElement>>();
    public WebSocketState? State => Socket?.State;
}
