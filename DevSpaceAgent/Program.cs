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

    public static bool DockerFailed = false;

    static async Task Main(string[] args)
    {
        if (!_Data.LoadConfig())
            throw new Exception("Failed to load config file.");

        try
        {
            DockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
            await DockerClient.System.PingAsync();
            Console.WriteLine("[Docker] Connected using direct socket access, not recommended.");
        }
        catch
        {
            DockerFailed = true;
        }

        if (DockerFailed)
        {
            try
            {
                DockerClient = new DockerClientConfiguration(new Uri("tcp://127.0.0.1:2376")).CreateClient();
                await DockerClient.System.PingAsync();
                Console.WriteLine("[Docker] Connected using secure socket.");
                DockerFailed = false;
            }
            catch
            {
                DockerFailed = true;
            }
        }

        if (DockerFailed)
            Console.WriteLine("[Docker] Failed to connect to socket!!!");

        SslContext context = new SslContext(SslProtocols.None, Certificate);
        context.CertificateValidationCallback = (x, s, t, b) => { return true; };
        AgentServer server = new AgentServer(context, IPAddress.Any, 5555);
        server.Start();

        await Task.Delay(-1);
    }
}
