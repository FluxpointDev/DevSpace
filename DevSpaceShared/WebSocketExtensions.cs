using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace DevSpaceShared;

/// <summary>
/// Arguments class
/// </summary>
public class Arguments
{
    // Variables
    private StringDictionary Parameters;

    // Constructor
    public Arguments(string[] Args)
    {
        Parameters = new StringDictionary();
        Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        string Parameter = null;
        string[] Parts;

        // Valid parameters forms:
        // {-,/,--}param{ ,=,:}((",')value(",'))
        // Examples: 
        // -param1 value1 --param2 /param3:"Test-:-work" 
        //   /param4=happy -param5 '--=nice=--'
        foreach (string Txt in Args)
        {
            // Look for new parameters (-,/ or --) and a
            // possible enclosed value (=,:)
            Parts = Spliter.Split(Txt, 3);

            switch (Parts.Length)
            {
                // Found a value (for the last parameter 
                // found (space separator))
                case 1:
                    if (Parameter != null)
                    {
                        if (!Parameters.ContainsKey(Parameter))
                        {
                            Parts[0] =
                                Remover.Replace(Parts[0], "$1");

                            Parameters.Add(Parameter, Parts[0]);
                        }
                        Parameter = null;
                    }
                    // else Error: no parameter waiting for a value (skipped)
                    break;

                // Found just a parameter
                case 2:
                    // The last parameter is still waiting. 
                    // With no value, set it to true.
                    if (Parameter != null)
                    {
                        if (!Parameters.ContainsKey(Parameter))
                            Parameters.Add(Parameter, "true");
                    }
                    Parameter = Parts[1];
                    break;

                // Parameter with enclosed value
                case 3:
                    // The last parameter is still waiting. 
                    // With no value, set it to true.
                    if (Parameter != null)
                    {
                        if (!Parameters.ContainsKey(Parameter))
                            Parameters.Add(Parameter, "true");
                    }

                    Parameter = Parts[1];

                    // Remove possible enclosing characters (",')
                    if (!Parameters.ContainsKey(Parameter))
                    {
                        Parts[2] = Remover.Replace(Parts[2], "$1");
                        Parameters.Add(Parameter, Parts[2]);
                    }

                    Parameter = null;
                    break;
            }
        }
        // In case a parameter is still waiting
        if (Parameter != null)
        {
            if (!Parameters.ContainsKey(Parameter))
                Parameters.Add(Parameter, "true");
        }
    }

    // Retrieve a parameter value if it exists 
    // (overriding C# indexer property)
    public string this[string Param]
    {
        get
        {
            return (Parameters[Param]);
        }
    }
}

public static class WebSocketExtensions
{

    public static async Task SendStringAsync(this WebSocketBase<System.Net.WebSockets.WebSocket> ws, string message, CancellationToken token = default)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task SendJsonAsync(this WebSocketBase<System.Net.WebSockets.WebSocket> ws, IWebSocketEvent json, CancellationToken token = default)
    {
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task SendJsonAsync(this System.Net.WebSockets.WebSocket ws, IWebSocketEvent json, CancellationToken token = default)
    {
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task RespondAsync(this WebSocketBase<System.Net.WebSockets.WebSocket> ws, string taskId, object json, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: \n" + Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented));

        string message = Newtonsoft.Json.JsonConvert.SerializeObject(new IWebSocketResponse<dynamic> { IsSuccess = true, TaskId = taskId, Data = json });
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task RespondFailAsync(this WebSocketBase<System.Net.WebSockets.WebSocket> ws, string taskId, CancellationToken token = default)
    {
        Console.WriteLine("Respond with: Fail");

        string message = JsonConvert.SerializeObject(new IWebSocketResponse<dynamic> { TaskId = taskId });
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task SendStringAsync(this WebSocketBase<ClientWebSocket> ws, string message, CancellationToken token = default)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);
    }

    public static async Task SendJsonAsync(this WebSocketBase<ClientWebSocket> ws, IWebSocketEvent json, CancellationToken token = default)
    {
        string message = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await ws.Socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, token);

    }


}
