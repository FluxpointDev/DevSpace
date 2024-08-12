namespace DevSpaceShared.WebSocket;
public class CommandEvent : IWebSocketTaskEvent
{
    public CommandEvent(bool wait) : base(wait ? EventType.CommandWait : EventType.Command)
    {

    }

    public string Command;
}
