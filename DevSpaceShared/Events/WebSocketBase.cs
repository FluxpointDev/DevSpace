using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace DevSpaceShared.WebSocket;

public class WebSocketBase<T> where T : System.Net.WebSockets.WebSocket
{
    public string? Id;
    public T? Socket;
    public ConcurrentDictionary<string, TaskCompletionSource<JToken>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JToken>>();
    public WebSocketState State => Socket.State;
}
