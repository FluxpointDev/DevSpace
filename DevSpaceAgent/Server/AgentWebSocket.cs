using DevSpaceShared.WebSocket;
using Newtonsoft.Json;

namespace DevSpaceAgent.Server;
public class AgentWebSocket
{
    public AgentSession Session;

    public async Task SendStringAsync(string message, CancellationToken token = default)
    {
        Session.SendTextAsync(message);
    }

    public async Task SendJsonAsync(IWebSocketEvent json, CancellationToken token = default)
    {
        string message = JsonConvert.SerializeObject(json);
        Session.SendTextAsync(message);
    }


    public async Task RespondAsync(string taskId, object json, bool noResponse = false, CancellationToken token = default)
    {

        if (!noResponse)
            Console.WriteLine("Respond with: \n" + Newtonsoft.Json.JsonConvert.SerializeObject(json, Formatting.Indented));
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json });
        Session.SendTextAsync(message);
    }

    public async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { TaskId = taskId });
        Session.SendTextAsync(message);
    }
}
