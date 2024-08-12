namespace DevSpaceShared.WebSocket;
public class CommandStreamEvent : IWebSocketEvent
{
    public CommandStreamEvent() : base(EventType.CommandStream)
    {

    }

    public string Command;

    public List<string> Stream;
}
