using DevSpaceShared;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Agents;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace DevSpaceWeb.Controllers.Misc;

public class EdgeController : Controller
{
    [HttpGet("/edge/ws")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        HttpContext context = HttpContext;

        string? EdgeId = context.Request.Headers["Edge-Id"];
        string? EdgeToken = context.Request.Headers["Edge-Key"];

        Console.WriteLine("Edge connection for: " + EdgeId);


        string? Response;
        if (string.IsNullOrEmpty(EdgeId))
        {
            Response = "Missing edge id.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (string.IsNullOrEmpty(EdgeToken))
        {
            Response = "Missing edge key.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (!ObjectId.TryParse(EdgeId, out ObjectId serverId))
        {
            Response = "Invalid edge id.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (!_DB.Servers.Cache.TryGetValue(serverId, out ServerData? server))
        {
            Response = "Unknown edge client.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (EdgeToken != server.AgentKey)
        {
            Response = "Invalid edge key.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (!_Data.EdgeAgents.TryGetValue(serverId, out Agents.EdgeAgent? edgeAgent))
        {
            Response = "Invalid edge client.";
            context.Response.Headers.ContentLength = Response.Length;
            return BadRequest(Response);
        }

        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            if (edgeAgent.WebSocket != null)
                edgeAgent.WebSocket.Dispose();

            edgeAgent.WebSocket = webSocket;

            Console.WriteLine("Edge connection handle");

            await Handle(webSocket, edgeAgent, cancellationToken);

            return Ok();
        }
        else
        {
            return Ok();
        }
    }

    private static async Task Handle(WebSocket webSocket, EdgeAgent edgeAgent, CancellationToken cancellationToken)
    {
        do
        {
            try
            {

                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);

                WebSocketReceiveResult result = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(ms.ToArray(), 0, result.Count);

                        JToken? payload = JsonConvert.DeserializeObject<JToken>(message);
                        if (payload == null)
                            return;

                        try
                        {
                            DevSpaceShared.WebSocket.EventType EventType = payload["Type"]!.ToObject<DevSpaceShared.WebSocket.EventType>();

                            Logger.LogMessage("WebSocket", "Edge Event: " + EventType.ToString(), LogSeverity.Info);


                            switch (EventType)
                            {
                                case EventType.TaskResponse:
                                    {
                                        IWebSocketResponse<dynamic> @event = payload.ToObject<IWebSocketResponse<dynamic>>()!;
                                        if (edgeAgent.TaskCollection.TryGetValue(@event.TaskId, out TaskCompletionSource<JToken>? task))
                                        {
                                            Logger.LogMessage("WebSocket", "Edge Got Response: " + @event.TaskId, LogSeverity.Info);
                                            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(payload, Formatting.Indented));
                                            if (@event.IsSuccess)
                                                task.SetResult(payload["Data"]);
                                            else
                                                task.SetCanceled();
                                        }
                                    }
                                    break;
                                case EventType.GetAgentStats:
                                    {
                                        AgentStatsResponse? Data = payload.ToObject<AgentStatsResponse>();
                                        if (Data == null)
                                            return;

                                        edgeAgent.Stats = Data;
                                    }
                                    break;

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                    }
                }
            }
            catch { }
        } while (!cancellationToken.IsCancellationRequested);
        Console.WriteLine("Edge Handle Removed");
    }
}
