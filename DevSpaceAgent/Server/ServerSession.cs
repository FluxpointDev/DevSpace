using DevSpaceAgent.Data;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevSpaceAgent.Server;

public class ServerSession : ISession
{
    public AgentSession? Session;

    public override async Task RespondAsync(string taskId, object json, bool noResponse = false, CancellationToken token = default)
    {
        if (!noResponse && json != null && _Data.Config.Options.LogAgentEvents)
            Console.WriteLine("Respond with: \n" + JsonConvert.SerializeObject(json, Formatting.Indented));

        DateTime Now = DateTime.UtcNow;
        //string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json });
        string message = System.Text.Json.JsonSerializer.Serialize(new IWebSocketResponse<dynamic>() { IsSuccess = true, TaskId = taskId, Data = json }, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            AllowTrailingCommas = true
        });

        TimeSpan Current = DateTime.UtcNow - Now;
        Console.WriteLine("Time: " + Current.TotalMilliseconds);
        Session.SendTextAsync(message);
    }

    public override async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic>() { TaskId = taskId });
        Session.SendTextAsync(message);
    }
}
