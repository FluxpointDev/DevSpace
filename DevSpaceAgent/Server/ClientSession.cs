using DevSpaceAgent.Data;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace DevSpaceAgent.Server;
public class ClientSession : ISession
{
    public WebSocket? Session;

    public override async Task RespondAsync(string taskId, object json, bool noResponse = false, CancellationToken token = default)
    {
        if (!noResponse && json != null && _Data.Config.Options.LogAgentEvents)
            Console.WriteLine("Respond with: \n" + JsonConvert.SerializeObject(json, Formatting.Indented));

        DateTime Now = DateTime.UtcNow;
        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json });
        byte[] Bytes = Encoding.UTF8.GetBytes(message);
        TimeSpan Current = DateTime.UtcNow - Now;
        Console.WriteLine("Send Time: " + Current.TotalMilliseconds);
        await Session.SendAsync(Bytes, WebSocketMessageType.Text, true, token);
    }

    public override async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { TaskId = taskId });
        await Session.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, token);
    }
}
