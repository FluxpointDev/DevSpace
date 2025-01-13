using DevSpaceAgent.Data;
using DevSpaceAgent.Server;
using Docker.DotNet;
using NetCoreServer;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace DevSpaceAgent;

public class Program
{
    //public static ConcurrentDictionary<string, WebSocketBase<System.Net.WebSockets.WebSocket>> Clients = new ConcurrentDictionary<string, WebSocketBase<System.Net.WebSockets.WebSocket>>();

    public static string CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

    public static DockerClient DockerClient;

    public static X509Certificate2 Certificate;

    static async Task Main(string[] args)
    {
        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config file.");

        bool DockerFailed = false;
        try
        {
            DockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
        }
        catch
        {
            DockerFailed = true;
        }

        try
        {
            await DockerClient.System.PingAsync();
        }
        catch
        {
            DockerFailed = true;
        }

        if (DockerFailed)
            throw new Exception("Docker is required to run on this server for Dev Space");

        SslContext context = new SslContext(SslProtocols.None, Certificate);
        context.CertificateValidationCallback = (x, s, t, b) => { return true; };
        AgentServer server = new AgentServer(context, IPAddress.Any, 5555);
        server.Start();

        await Task.Delay(-1);
    }
}
