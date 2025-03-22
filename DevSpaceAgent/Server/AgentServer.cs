using DevSpaceAgent.Client;
using DevSpaceAgent.Data;
using DevSpaceShared.Data;
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

    }

    protected override void OnReceivedRequestHeader(HttpRequest request)
    {
        if (request.Url == "/")
        {
            string Ip = Socket.RemoteEndPoint.ToString().Split(':').First();
            if (!_Data.Config.AllowedIPs.Contains(Ip))
            {
                Close();
                return;
            }

            string ClientKey = "";
            long HeaderCount = request.Headers;
            if (HeaderCount < 30)
            {
                int Count = 0;
                while (Count != HeaderCount)
                {
                    (string, string) Header = request.Header(Count);
                    if (Header.Item1 == "Authorization")
                    {
                        ClientKey = Header.Item2;
                        break;
                    }

                    Count += 1;
                }
            }

            if (string.IsNullOrEmpty(ClientKey) || ClientKey != _Data.Config.AgentKey)
            {
                Close();
                return;
            }

            base.OnReceivedRequestHeader(request);
        }

    }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        string path = request.Url.Substring(1);
        if (!string.IsNullOrEmpty(path))
        {
            if (request.Method != "GET")
                SendResponseAsync(Response.MakeErrorResponse(400, "Unsupported HTTP method: " + request.Method));
            else
            {
                bool IsWhitelisted = false;
                string Ip = Socket.RemoteEndPoint.ToString().Split(':').First();

                if (!_Data.Config.AllowedIPs.Any())
                    IsWhitelisted = true;
                else
                    IsWhitelisted = _Data.Config.AllowedIPs.Contains(Ip);

                if (!IsWhitelisted)
                {
                    Console.WriteLine("IP Blocked");
                    SendResponseAsync(Response.MakeErrorResponse(401, "You are not authorized."));
                    return;
                }

                switch (path)
                {
                    case "discover":

                        SendJsonResponseAsync(new DiscoverAgentInfo
                        {
                            Id = _Data.Config.AgentId,
                            Version = Program.Version
                        });
                        return;
                    default:
                        {
                            string ClientKey = "";
                            long HeaderCount = request.Headers;
                            if (HeaderCount < 30)
                            {
                                int Count = 0;

                                while (Count != HeaderCount)
                                {
                                    (string, string) Header = request.Header(Count);
                                    if (Header.Item1 == "Authorization")
                                    {
                                        ClientKey = Header.Item2;
                                        break;
                                    }

                                    Count += 1;
                                }
                            }

                            if (string.IsNullOrEmpty(ClientKey) || ClientKey != _Data.Config.AgentKey)
                            {
                                SendResponseAsync(Response.MakeErrorResponse(401, "You are not authorized."));
                                return;
                            }

                            switch (path)
                            {
                                case "setup":
                                    if (_Data.Config.AllowedIPs.Add(Ip))
                                        _Data.Config.Save();

                                    SendResponseAsync(Response.MakeOkResponse());

                                    return;
                                default:
                                    SendResponseAsync(Response.MakeErrorResponse(400, "Unknown request."));
                                    return;
                            }
                        }
                }
            }
            return;
        }

        base.OnReceivedRequest(request);
    }

    public void SendJsonResponseAsync(object data)
    {
        SendResponseAsync(Response.MakeGetResponse(Newtonsoft.Json.JsonConvert.SerializeObject(data), "application/json"));
    }

    public override bool OnWsConnecting(HttpRequest request, HttpResponse response)
    {
        Console.WriteLine("Connecting...");

        string Ip = Socket.RemoteEndPoint.ToString().Split(':').First();
        if (!_Data.Config.AllowedIPs.Contains(Ip))
        {
            Console.WriteLine("IP Blocked");
            this.Disconnect();

            return false;
        }

        string ClientKey = "";
        long HeaderCount = request.Headers;
        if (HeaderCount < 30)
        {
            int Count = 0;
            while (Count != HeaderCount)
            {
                (string, string) Header = request.Header(Count);
                if (Header.Item1 == "Authorization")
                {
                    ClientKey = Header.Item2;
                    break;
                }

                Count += 1;
            }
        }

        if (ClientKey == _Data.Config.AgentKey)
        {
            Console.WriteLine("Key match");
            return true;
        }

        this.Disconnect();

        Console.WriteLine("Key not match");

        return false;
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