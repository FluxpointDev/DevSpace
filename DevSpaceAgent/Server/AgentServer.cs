using DevSpaceAgent.Client;
using DevSpaceShared.WebSocket;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace DevSpaceAgent.Server;
public class AgentSession : WssSession
{
    public AgentSession(WssServer server) : base(server) { }

    public AgentWebSocket AgentWebSocket { get; set; }


    public override async void OnWsConnected(HttpRequest request)
    {
        AgentWebSocket = new AgentWebSocket { Session = this };
        Console.WriteLine($"WebSocket session with Id {Id} connected!");
        AgentWebSocket.SendJsonAsync(new ValidateCertEvent() { CertHash = Program.Certificate.GetCertHashString() }, default, true);

    }

    public override void OnWsDisconnected()
    {
        Console.WriteLine($"WebSocket session with Id {Id} disconnected!");
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        ServerEventHandler.RecieveAsync(AgentWebSocket, buffer, offset, size);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"WebSocket session caught an error with code {error}");
    }


}

class AgentServer : WssServer
{
    public AgentServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

    protected override SslSession CreateSession() { return new AgentSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"WebSocket server caught an error with code {error}");
    }
}