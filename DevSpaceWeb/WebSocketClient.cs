using DevSpaceShared.WebSocket;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace DevSpaceWeb;

public class ValidateCert
{
    public string Cert;
}
public class DevSpaceClient
{
    public bool FirstCertVaid;
    public bool IsCertValid;
    public ChatClient Client;
    public ConcurrentDictionary<string, TaskCompletionSource<JToken>> TaskCollection = new ConcurrentDictionary<string, TaskCompletionSource<JToken>>();
}
public class ChatClient : WssClient
{
    public ChatClient(SslContext context, string address, int port) : base(context, address, port) { }

    public DevSpaceClient WebSocket = new DevSpaceClient();
    public ValidateCert ValidateCert;
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
        request.SetHeader("Host", "localhost");
        request.SetHeader("Origin", "http://127.0.0.1");
        request.SetHeader("Upgrade", "websocket");
        request.SetHeader("Connection", "Upgrade");
        request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
        request.SetHeader("Sec-WebSocket-Protocol", "");
        request.SetHeader("Sec-WebSocket-Version", "13");
        request.SetBody();

    }

    public override void OnWsConnected(NetCoreServer.HttpResponse response)
    {
        WebSocket = new DevSpaceClient { Client = this };
        Console.WriteLine($"Chat WebSocket client connected a new session with Id {Id}");
    }

    public override void OnWsDisconnected()
    {
        Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        Console.WriteLine("Got MESSAGE");
        WebSocketMessage(WebSocket, Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();
        Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat WebSocket client caught an error with code {error}");
    }

    private bool _stop;

    private async Task WebSocketMessage(DevSpaceClient WebSocket, string json)
    {
        JToken payload = JsonConvert.DeserializeObject<JToken>(json);

        EventType EventType = payload!["type"]!.ToObject<EventType>();

        Console.WriteLine("Event: " + EventType.ToString());

        try
        {
            //if (Client.Config.Debug.LogWebSocketFull)
            //{
            //    switch (payload["type"].ToString())
            //    {
            //        case "Ready":
            //        case "UserUpdate":
            //        case "ChannelStartTyping":
            //        case "ChannelStopTyping":
            //            break;
            //        default:
            //            Client.Logger.LogJson("WebSocket Response Json", json);
            //            break;
            //    }

            //}

            switch (EventType)
            {
                case EventType.ValidateCert:
                    {
                        ValidateCertEvent @event = payload.ToObject<ValidateCertEvent>()!;
                        Console.WriteLine("Client Validate Cert: " + @event.CertHash);
                        Console.WriteLine("With " + WebSocket.Client.ValidateCert.Cert);
                        if (WebSocket.Client.ValidateCert.Cert == @event.CertHash)
                        {
                            if (WebSocket.FirstCertVaid)
                            {
                                Console.WriteLine("Validated: " + WebSocket.IsCertValid);
                                WebSocket.IsCertValid = true;
                            }
                            else
                            {
                                WebSocket.FirstCertVaid = true;

                                WebSocket.Client.SendTextAsync(json);
                            }


                        }
                    }
                    break;
                case EventType.TaskResponse:
                    {
                        IWebSocketResponseEvent<dynamic> @event = payload.ToObject<IWebSocketResponseEvent<dynamic>>()!;
                        Console.WriteLine("Got Response: " + @event.TaskId);
                        if (WebSocket.TaskCollection.TryGetValue(@event.TaskId, out var task))
                        {
                            if (@event.IsFail)
                                task.SetCanceled();
                            else
                                task.SetResult(payload);
                        }
                    }
                    break;

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket Event {payload["type"].ToString()} Error ");
            Console.WriteLine(ex);
        }
    }

    public async Task<T?> RecieveJsonAsync<T>(IWebSocketTaskEvent json, CancellationToken token = default) where T : class
    {
        if (!WebSocket.IsCertValid)
        {
            Console.WriteLine("Invalid Cert: " + json.type.ToString());
            return null;
        }



        json.TaskId = Guid.NewGuid().ToString();
        var message = JsonConvert.SerializeObject(json);

        var tcs = new TaskCompletionSource<JToken>();
        WebSocket.TaskCollection.TryAdd(json.TaskId, tcs);
        SendTextAsync(message);

        var result = await tcs.Task.WaitAsync(new TimeSpan(0, 0, 30), token);
        if (result == null)
        {
            return null;
        }
        Console.WriteLine("Task Success: " + tcs.Task.IsCompletedSuccessfully);

        var Response = result.ToObject<IWebSocketResponseEvent<T>>();

        if (Response.IsFail)
            return null;


        return Response.Data;
    }
}





