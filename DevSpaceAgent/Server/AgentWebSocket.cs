using DevSpaceShared.WebSocket;
using Newtonsoft.Json;

namespace DevSpaceAgent.Server;
public class AgentWebSocket
{
    public bool IsCertValid;
    public AgentSession Session;

    public async Task SendStringAsync(string message, CancellationToken token = default)
    {
        if (!IsCertValid)
        {
            Console.WriteLine("Invalid Cert: " + message);
            return;
        }
        Session.SendTextAsync(message);
    }

    public async Task SendJsonAsync(IWebSocketEvent json, CancellationToken token = default, bool bypassCertCheck = false)
    {
        if (!bypassCertCheck && !IsCertValid)
        {
            Console.WriteLine("Invalid Cert: " + json.type.ToString());
            return;
        }
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        Console.WriteLine("Send Cert Message");
        bool Check = Session.SendTextAsync(message);
        Console.WriteLine("Send: " + Check);
    }


    public async Task RespondAsync(string taskId, object json, CancellationToken token = default)
    {
        if (!IsCertValid)
        {
            Console.WriteLine("Invalid Cert: " + json.GetType().Name);
            return;
        }
        Console.WriteLine("Respond with: \n" + Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented));
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(new IWebSocketResponseEvent<dynamic> { TaskId = taskId, Data = json });
        Session.SendTextAsync(message);
    }

    public async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {
        if (!IsCertValid)
        {
            Console.WriteLine("Invalid Cert: Respond With Fail");
            return;
        }
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponseEvent<dynamic> { TaskId = taskId, IsFail = true });
        Session.SendTextAsync(message);
    }
}
