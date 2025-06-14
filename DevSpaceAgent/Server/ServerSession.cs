using DevSpaceAgent.Data;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;

namespace DevSpaceAgent.Server;

public class ServerSession : ISession
{
    public AgentSession? Session;

    public override async Task RespondAsync(string taskId, object json, bool noResponse = false, CancellationToken token = default)
    {
        if (!noResponse && json != null && _Data.Config.Options.LogAgentEvents)
            Console.WriteLine("Respond with: \n" + JsonConvert.SerializeObject(json, Formatting.Indented));

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json });
        Session.SendTextAsync(message);
    }

    public override async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { TaskId = taskId });
        Session.SendTextAsync(message);
    }
}
