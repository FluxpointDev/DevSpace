using DevSpaceShared.Data;
using DevSpaceShared.Responses;
using DevSpaceShared.Services;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Data.Servers;
using Radzen;
using System.Net;

namespace DevSpaceWeb.Agents;

public class ClientAgent : IAgent
{
    public WebSocketClient WebSocket { get; set; }

    public bool StopReconnect;

    public bool Reconnect;

    public DiscoverAgentInfo? Discover;

    public required ServerData Server;

    public async Task DiscoverAsync(string host, short port)
    {
        HttpRequestMessage Req = new HttpRequestMessage(HttpMethod.Get, "https://" + host + ":" + port + "/discover");
        Req.Headers.Add("Connection", "Upgrade");
        HttpResponseMessage? Response = null;
        try
        {
            Response = await Program.AgentDiscoverHttp.SendAsync(Req);
        }
        catch (HttpRequestException he)
        {
            switch (he.HttpRequestError)
            {
                case HttpRequestError.ConnectionError:
                case HttpRequestError.NameResolutionError:
                case HttpRequestError.SecureConnectionError:
                    {
                        Error = WebSocketErrorType.NoConnection;
                    }
                    return;
            }
        }
        catch { }
        if (Response == null)
        {
            Error = WebSocketErrorType.ServerError;
            return;
        }
        if (!Response.IsSuccessStatusCode)
        {
            switch (Response.StatusCode)
            {
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.GatewayTimeout:
                    Error = WebSocketErrorType.NoConnection;
                    break;
                case HttpStatusCode.BadRequest:
                    Error = WebSocketErrorType.AgentUnsupported;
                    break;
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    Error = WebSocketErrorType.AuthFailed;
                    break;
                default:
                    Error = WebSocketErrorType.AgentError;
                    break;
            }

            return;
        }
        try
        {
            using (Stream responseContent = await Response.Content.ReadAsStreamAsync())
                Discover = await System.Text.Json.JsonSerializer.DeserializeAsync<DiscoverAgentInfo>(responseContent);
        }
        catch
        {
            Error = WebSocketErrorType.ServerError;
        }

        if (Discover == null)
            Error = WebSocketErrorType.ServerError;
    }

    public override bool IsConnected => WebSocket != null && WebSocket.IsConnected;

    public override async Task Connect(string host, short port, string key, bool reconnect = false)
    {
        await DiscoverAsync(host, port);

        if (Error.HasValue && Error != WebSocketErrorType.NoConnection)
            return;

        if (Discover == null)
        {
            if (!StopReconnect && reconnect)
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                await Connect(host, port, key, true);
            }
            return;
        }

        if (Server != null && Discover.Id != Server.AgentId)
        {
            Error = WebSocketErrorType.AgentError;
            return;
        }
        Error = null;

        IPAddress? address = null;
        if (!IPAddress.TryParse(host, out address))
        {
            IPHostEntry? Host = null;
            try
            {
                Host = Dns.GetHostEntry(host);
            }
            catch { }
            if (Host != null)
                address = Host.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
        }

        if (address == null)
        {
            Error = WebSocketErrorType.NoConnection;
            if (!StopReconnect && Reconnect)
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                await Connect(host, port, key, true);
            }
            return;
        }

        Logger.LogMessage($"Connecting to {host}:{port}", LogSeverity.Info);
        WebSocket = new WebSocketClient(key, this, new DnsEndPoint(host, port, host.Contains(":") ? System.Net.Sockets.AddressFamily.InterNetworkV6 : System.Net.Sockets.AddressFamily.InterNetwork));

        Logger.LogMessage("Client connecting...", LogSeverity.Debug);
        bool Connected = WebSocket.ConnectAsync();

        if (!Connected)
            Error = WebSocketErrorType.NoConnection;
    }

    public override async Task Disconnect()
    {

    }

    public async Task<SocketResponse<T?>> RunJsonAsync<T>(NotificationService notification, IWebSocketTask json, Action<SocketResponse<T?>>? action = null, CancellationToken token = default) where T : class
    {
        SocketResponse<T?> Result = await RecieveJsonAsync<T>(json, token);
        if (Result.IsSuccess)
        {
            action?.Invoke(Result);
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

    public override Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
        => WebSocket != null ? WebSocket.RecieveJsonAsync<T>(json, token) : DummyResponse<T>();

    private async Task<SocketResponse<T?>> DummyResponse<T>()
    {
        return new SocketResponse<T?> { Error = ClientError.Exception };
    }
}
