using DevSpaceAgent.Client;
using DevSpaceShared;
using DevSpaceShared.Services;
using Docker.DotNet.Models;
using System.Net.WebSockets;
using System.Text;

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

    public ClientWebSocket WebSocket;

    public int ReconnectCount = 5;

    public override async Task Connect(string host, short port, string key, bool reconnect = false)
    {
        if (WebSocket != null)
        {
            try
            {
                WebSocket.Dispose();
            }
            catch { }
        }

        WebSocket = new ClientWebSocket();
        WebSocket.Options.SetRequestHeader("Edge-Id", Id);
        WebSocket.Options.SetRequestHeader("Edge-Key", Key);

        try
        {
            await WebSocket.ConnectAsync(new Uri($"wss://{Host}:{Port}/edge/ws"), CancellationToken.None);
        }
        catch { }
        bool StatsSent = false;
        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);

        WebSocketReceiveResult? result = null;

        if (WebSocket.State == WebSocketState.Open)
            ReconnectCount = 5;

        while (WebSocket.State == WebSocketState.Open)
        {
            if (!StatsSent)
            {
                StatsSent = true;
                if (Program.DockerClient != null)
                {
                    SystemInfoResponse HostInfo = await Program.DockerClient.System.GetSystemInfoAsync();
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(HostInfo.Swarm, Newtonsoft.Json.Formatting.Indented));
                    AgentStatsResponse Stats = await AgentStatsResponse.Create(Program.Version, Program.DockerClient, HostInfo);

                    _ = WebSocket.SendAsync(Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(Stats, AgentJsonOptions.Options)), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        result = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Server closed the connection.");
                        await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {

                        _ = ServerEventHandler.RecieveAsync(new ClientSession
                        {
                            Session = WebSocket
                        }, ms.ToArray(), 0, result.Count);
                    }
                }
            }
            catch
            {

            }
        }

        if (ReconnectCount != 15)
            ReconnectCount += 5;

        Thread.Sleep(TimeSpan.FromSeconds(ReconnectCount));

        Logger.LogMessage("WebSocket", $"Reconnecting to {Host}:{Port}", LogSeverity.Info);

        await Connect(host, port, key, reconnect);
    }
}
