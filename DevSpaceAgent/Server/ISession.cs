namespace DevSpaceAgent.Server;

public class ISession
{
    public virtual async Task RespondAsync(string taskId, object json, bool noResponse = false, CancellationToken token = default)
    {

    }

    public virtual async Task RespondFailAsync(string taskId, CancellationToken token = default)
    {

    }
}
