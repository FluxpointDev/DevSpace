using DevSpaceShared.Events.Docker;

namespace DevSpaceAgent.Server;
public static class DockerHandler
{
    public static async Task<object?> Run(DockerEvent @event)
    {
        switch (@event.DockerType)
        {

        }

        return null;
    }
}
