using DevSpaceShared.Responses;
using DevSpaceShared.Services;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Servers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzen;
using System.Text;

namespace DevSpaceWeb.Agents;

public class EdgeAgent : IAgent
{
    public override bool IsConnected => _Data.EdgeAgents.TryGetValue(Server.Id, out EdgeAgent? edgeAgent) && edgeAgent.WebSocket != null && edgeAgent.WebSocket.State == System.Net.WebSockets.WebSocketState.Open;

    public required ServerData Server;

    public System.Net.WebSockets.WebSocket WebSocket { get; set; }

    public override async Task Connect(string host, short port, string key, bool reconnect = false)
    {
        if (!_Data.EdgeAgents.ContainsKey(Server.Id))
            _Data.EdgeAgents.TryAdd(Server.Id, this);
        else
            _Data.EdgeAgents[Server.Id] = this;
    }

    public override async Task Disconnect()
    {
        if (_Data.EdgeAgents.ContainsKey(Server.Id))
            _Data.EdgeAgents.Remove(Server.Id);

        if (WebSocket != null)
            WebSocket.Dispose();
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

    public override async Task<SocketResponse<T?>> RecieveJsonAsync<T>(IWebSocketTask json, CancellationToken token = default) where T : class
    {
        json.TaskId = Guid.NewGuid().ToString();
        Console.WriteLine("Task: " + json.TaskId);
        TaskCompletionSource<JToken> tcs = new TaskCompletionSource<JToken>();
        TaskCollection.TryAdd(json.TaskId, tcs);
        string message = JsonConvert.SerializeObject(json);

        byte[] encoded = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
        WebSocket.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, token);

        Console.WriteLine("Sending");
        JToken? result = null;
        try
        {
            result = await tcs.Task.WaitAsync(new TimeSpan(0, 0, 30), token);
        }
        catch
        {
            return new SocketResponse<T?> { Error = ClientError.Timeout };
        }
        Console.WriteLine("Wait done");

        TaskCollection.TryRemove(json.TaskId, out _);
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
