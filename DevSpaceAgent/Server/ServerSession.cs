using DevSpaceAgent.Data;
using DevSpaceShared;
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

        string message = System.Text.Json.JsonSerializer.Serialize(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json }, AgentJsonOptions.Options);

        Session.SendTextAsync(message);
    }

    public override async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        string message = System.Text.Json.JsonSerializer.Serialize(new IWebSocketResponse<dynamic>() { TaskId = taskId }, AgentJsonOptions.Options);
        Session.SendTextAsync(message);
    }
}
