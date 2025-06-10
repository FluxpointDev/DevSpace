using DevSpaceShared.Services;

namespace DevSpaceAgent.Server;

public class EdgeClient : IAgent
{
    public EdgeClient(string host, short port, string id, string key)
    {
        Host = host;
        Port = port;
        Id = id;
        Key = key;
    }

    public override bool IsConnected => false;

    public string Host;
    public short Port;
    public string Id;
    public string Key;


    public override async Task Connect(string host, short port, string key, bool reconnect = false)
    {

    }
}