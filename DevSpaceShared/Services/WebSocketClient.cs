﻿using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace DevSpaceShared.Services;

public class WebSocketClient : WssClient
{
    public WebSocketClient(string key, IAgent agent, DnsEndPoint host) : base(new SslContext(SslProtocols.None, (e, b, l, m) =>
    {
        if (b != null && m != System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
        {
            return true;
        }
        return false;
    }), host)
    {
        Key = key;
        Agent = agent;
    }

    public string Key;

    public IAgent Agent;

    public void DisconnectAndStop()
    {
        _stop = true;
        CloseAsync(1000);
        while (IsConnected)
            Thread.Yield();
    }

    public override void OnWsConnecting(HttpRequest request)
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

    public override void OnWsConnected(HttpResponse response)
    {
        Logger.LogMessage("WebSocket", $"WebSocket client connected a new session with Id {Id}", LogSeverity.Info);
    }

    public override void OnWsDisconnected()
    {
        //Logger.LogMessage("WebSocket", $"WebSocket client disconnected a session with Id {Id}", LogSeverity.Info);
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        Logger.LogMessage("WebSocket", "Got MESSAGE", LogSeverity.Info);
        _ = WebSocketMessage(Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
    }

    protected override void OnDisconnected()
    {
        Logger.LogMessage("WebSocket", $"WebSocket client disconnected a session with Id {Id}", LogSeverity.Info);
        base.OnDisconnected();
        Thread.Sleep(TimeSpan.FromSeconds(15));

        Logger.LogMessage("WebSocket", $"Reconnecting to {Address}:{Port}", LogSeverity.Info);
        if (!_stop)
        {
            ConnectAsync();
        }
    }

    protected override void OnError(SocketError error)
    {
        Logger.LogMessage("WebSocket", $"WebSocket client caught an error with code {error}", LogSeverity.Warn);
    }

    private bool _stop;

    private async Task WebSocketMessage(string json)
    {
        JsonDocument? payload = JsonDocument.Parse(json);

        if (payload == null)
            return;

        EventType EventType = (EventType)payload.RootElement.GetProperty("Type").GetInt32();

        Logger.LogMessage("WebSocket", "Event: " + EventType.ToString(), LogSeverity.Info);

        try
        {
            switch (EventType)
            {
                case EventType.TaskResponse:
                    {
                        IWebSocketResponse<dynamic> @event = payload.Deserialize<IWebSocketResponse<dynamic>>(AgentJsonOptions.Options)!;
                        if (Agent.TaskCollection.TryGetValue(@event.TaskId, out TaskCompletionSource<JsonElement>? task))
                        {
                            Logger.LogMessage("WebSocket", "Got Response: " + @event.TaskId, LogSeverity.Info);

                            if (@event.IsSuccess)
                                task.SetResult(payload.RootElement.GetProperty("Data"));
                            else
                                task.SetCanceled();
                        }
                    }
                    break;
                case EventType.GetAgentStats:
                    {
                        AgentStatsResponse? Data = payload.Deserialize<AgentStatsResponse>(AgentJsonOptions.Options);
                        if (Data == null)
                            return;

                        Agent.Stats = Data;
                    }
                    break;

            }

        }
        catch (Exception ex)
        {
            Logger.LogMessage("WebSocket", $"WebSocket Event {EventType.ToString()} Error", LogSeverity.Error);
            Logger.LogMessage("WebSocket", ex.ToString(), LogSeverity.Error);
        }
    }

    public async Task<SocketResponse<T?>> RecieveJsonAsync<T, InputJson>(InputJson json, CancellationToken token = default) where T : class where InputJson : IWebSocketTask
    {
        json.TaskId = Guid.NewGuid().ToString();
        TaskCompletionSource<JsonElement> tcs = new TaskCompletionSource<JsonElement>();
        Agent.TaskCollection.TryAdd(json.TaskId, tcs);
        string message = System.Text.Json.JsonSerializer.Serialize(json, AgentJsonOptions.Options);
        SendTextAsync(message);

        JsonElement? result = null;
        try
        {
            result = await tcs.Task.WaitAsync(new TimeSpan(0, 0, 30), token);
        }
        catch
        {
            return new SocketResponse<T?> { Error = ClientError.Timeout };
        }
        Agent.TaskCollection.TryRemove(json.TaskId, out _);
        if (result == null)
        {
            return new SocketResponse<T?> { Error = ClientError.Timeout };
        }

        SocketResponse<T?>? Response = result.Value.Deserialize<SocketResponse<T?>>(AgentJsonOptions.Options);
        if (Response == null)
            return new SocketResponse<T?> { Error = ClientError.JsonError };


        return Response;
    }
}