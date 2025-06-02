using DevSpaceShared.WebSocket;

namespace DevSpaceShared.Agent;
public class AgentOptions
{
    public bool LogDockerEvents { get; set; }
    public bool LogAgentEvents { get; set; }
    public void Update(AgentOptionsUpdate data)
    {
        Console.WriteLine("--- Agent Options ---");
        if (data.LogDockerEvents.HasValue)
        {
            Console.WriteLine("Log Docker Events: " + data.LogDockerEvents.Value);
            LogDockerEvents = data.LogDockerEvents.Value;
        }

        if (data.LogAgentEvents.HasValue)
        {
            Console.WriteLine("Log Agent Events: " + data.LogAgentEvents.Value);
            LogAgentEvents = data.LogAgentEvents.Value;
        }

        Console.WriteLine("--- --- --- --- ---");
    }
}
public class AgentOptionsUpdate : IWebSocketTask
{
    public AgentOptionsUpdate() : base(EventType.UpdateAgentOptions)
    {

    }

    public bool? LogDockerEvents { get; set; }
    public bool? LogAgentEvents { get; set; }
}
