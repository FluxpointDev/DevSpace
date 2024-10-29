using DevSpaceShared.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DevSpaceShared.Events.Docker;

public class DockerEvent : IWebSocketTaskEvent
{
    public DockerEvent(DockerEventType type, string? resourceId = null) : base(EventType.Docker)
    {
        DockerType = type;
        ResourceId = resourceId;
    }

    public string? ResourceId { get; set; }
    public DockerEventType DockerType { get; set; }
}
public enum DockerEventType
{

}