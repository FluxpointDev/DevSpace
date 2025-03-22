using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzen;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace DevSpaceWeb.WebSocket;

public class WebSocketBase
{
    [JsonIgnore]
    public WebSocketClient Client;
    [JsonIgnore]
    public ConcurrentDictionary<string, TaskCompletionSource<JToken>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JToken>>();
}
public class WebSocketClient : WssClient
{
    public WebSocketClient(SslContext context, string address, int port) : base(context, address, port) { }

    public WebSocketBase WebSocket = new WebSocketBase();
    public string Key;
    public void DisconnectAndStop()
    {
        _stop = true;
        CloseAsync(1000);
        while (IsConnected)
            Thread.Yield();
    }

    public override void OnWsConnecting(NetCoreServer.HttpRequest request)
    {
        request.SetBegin("GET", "/");
        request.SetHeader("Upgrade", "websocket");
        request.SetHeader("Connection", "Upgrade");
        request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
        request.SetHeader("Sec-WebSocket-Protocol", "devspace");
        request.SetHeader("Sec-WebSocket-Version", "13");
        request.SetHeader("Authorization", Key);
        request.SetBody();

        Logger.LogMessage("WebSocket", "Connecting websocket", LogSeverity.Info);
    }

    public override void OnWsConnected(NetCoreServer.HttpResponse response)
    {
        WebSocket = new WebSocketBase { Client = this };
        Logger.LogMessage("WebSocket", $"WebSocket client connected a new session with Id {Id}", LogSeverity.Info);
    }

    public override void OnWsDisconnected()
    {
        //Logger.LogMessage("WebSocket", $"WebSocket client disconnected a session with Id {Id}", LogSeverity.Info);
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        Logger.LogMessage("WebSocket", "Got MESSAGE", LogSeverity.Info);
        WebSocketMessage(WebSocket, Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
    }


    protected override void OnDisconnected()
    {
        Logger.LogMessage("WebSocket", $"WebSocket client disconnected a session with Id {Id}", LogSeverity.Info);
        base.OnDisconnected();
        Thread.Sleep(TimeSpan.FromSeconds(15));

        Logger.LogMessage("WebSocket", $"Reconnecting to {Address}", LogSeverity.Info);
        if (!_stop)
            ConnectAsync();
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogMessage("WebSocket", $"WebSocket client caught an error with code {error}", LogSeverity.Warn);
    }

    private bool _stop;

    private async Task WebSocketMessage(WebSocketBase WebSocket, string json)
    {
        JToken payload = JsonConvert.DeserializeObject<JToken>(json);

        EventType EventType = payload!["Type"]!.ToObject<EventType>();

        Logger.LogMessage("WebSocket", "Event: " + EventType.ToString(), LogSeverity.Info);

        try
        {
            switch (EventType)
            {
                case EventType.TaskResponse:
                    {
                        IWebSocketResponse<dynamic> @event = payload.ToObject<IWebSocketResponse<dynamic>>()!;
                        if (WebSocket.TaskCollection.TryGetValue(@event.TaskId, out TaskCompletionSource<JToken>? task))
                        {
                            Logger.LogMessage("WebSocket", "Got Response: " + @event.TaskId, LogSeverity.Info);
                            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(payload, Formatting.Indented));
                            if (@event.IsSuccess)
                                task.SetResult(payload["Data"]);
                            else
                                task.SetCanceled();
                        }
                    }
                    break;

            }

        }
        catch (Exception ex)
        {
            Logger.LogMessage("WebSocket", $"WebSocket Event {payload["type"].ToString()} Error", LogSeverity.Error);
            Logger.LogMessage("WebSocket", ex.ToString(), LogSeverity.Error);
        }
    }

    public async Task<SocketResponse<T?>> RunJsonAsync<T>(NotificationService notification, IWebSocketTask json, Action<SocketResponse<T?>>? action = null, CancellationToken token = default) where T : class
    {
        var Result = await RecieveJsonAsync<T>(json, token);
        if (Result.IsSuccess)
        {
            if (action != null)
                action.Invoke(Result);
        }
        else
        {
            switch (Result.Error)
            {
                case ClientError.Timeout:
                    Result.Message = "Request timed out.";
                    break;
                case ClientError.JsonError:
                    Result.Message = "Failed to parse data.";
                    break;
                case ClientError.CertError:
                    Result.Message = "Failed server validation.";
                    break;
                case ClientError.AuthError:
                    Result.Message = "Failed server authentication.";
                    break;
            }
            if (string.IsNullOrEmpty(Result.Message))
                Result.Message = "Failed to send request.";

            notification.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Duration = 40000,
                Summary = "Error",
                Detail = Result.Message

            });
        }

        return Result;
    }

    public async Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
    {
        json.TaskId = Guid.NewGuid().ToString();
        TaskCompletionSource<JToken> tcs = new TaskCompletionSource<JToken>();
        WebSocket.TaskCollection.TryAdd(json.TaskId, tcs);
        string message = JsonConvert.SerializeObject(json);
        SendTextAsync(message);

        JToken result = await tcs.Task.WaitAsync(new TimeSpan(0, 0, 10), token);
        WebSocket.TaskCollection.TryRemove(json.TaskId, out _);
        if (result == null)
        {
            return new SocketResponse<T?> { Error = ClientError.Timeout };
        }

        SocketResponse<T?>? Response = result.ToObject<SocketResponse<T?>>();

        if (Response == null)
            return new SocketResponse<T?> { Error = ClientError.JsonError };


        return Response;
    }
}





