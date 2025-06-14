using DevSpaceAgent.Client;
using DevSpaceShared.Services;
using System.Net.WebSockets;

namespace DevSpaceAgent.Server;

public class EdgeClient : IAgent
{
    public EdgeClient(string host, short port, string id, string key)
    {
        Host = host;
        Port = port;
        Id = id;
        Key = key;
    }

    public override bool IsConnected => false;

    public string Host;
    public short Port;
    public string Id;
    public string Key;

    private ClientWebSocket WebSocket;

    public override async Task Connect(string host, short port, string key, bool reconnect = false)
    {
        if (WebSocket == null)
        {
            WebSocket = new ClientWebSocket();
            WebSocket.Options.SetRequestHeader("Edge-Id", Id);
            WebSocket.Options.SetRequestHeader("Edge-Key", Key);
        }

        await WebSocket.ConnectAsync(new Uri($"wss://{Host}:{Port}/edge/ws"), CancellationToken.None);

        byte[] receiveBuffer = new byte[1024];
        while (WebSocket.State == WebSocketState.Open)
        {

            try
            {
                WebSocketReceiveResult result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Server closed the connection.");
                    await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                    ServerEventHandler.RecieveAsync(new ClientSession
                    {
                        Session = WebSocket
                    }, receiveBuffer, 0, result.Count);
                }
            }
            catch
            {

            }

        }

        Thread.Sleep(TimeSpan.FromSeconds(15));

        Logger.LogMessage("WebSocket", $"Reconnecting to {Host}:{Port}", LogSeverity.Info);

        await Connect(host, port, key, reconnect);
    }
}
