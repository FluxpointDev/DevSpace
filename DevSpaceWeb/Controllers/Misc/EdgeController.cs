using DevSpaceShared.WebSocket;
using DevSpaceWeb.Agents;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Http.Features;
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
        IHttpResponseBodyFeature? bufferingFeature = context.Features.Get<IHttpResponseBodyFeature>();
        bufferingFeature?.DisableBuffering();
        context.Response.Headers.ContentLength = 0;
        string? EdgeId = context.Request.Headers["Edge-Id"];
        string? EdgeToken = context.Request.Headers["Edge-Key"];



        Console.WriteLine("Edge connection for: " + EdgeId);

        if (string.IsNullOrEmpty(EdgeId))
            return BadRequest("Missing edge id.");

        if (string.IsNullOrEmpty(EdgeToken))
            return BadRequest("Missing edge token.");

        if (!ObjectId.TryParse(EdgeId, out ObjectId serverId))
            return BadRequest("Invalid edge id.");

        if (!_DB.Servers.Cache.TryGetValue(serverId, out ServerData? server))
            return BadRequest("Unknown edge client.");

        if (EdgeToken != server.AgentKey)
            return BadRequest("Invalid edge token.");

        if (!_Data.EdgeAgents.TryGetValue(serverId, out Agents.EdgeAgent? edgeAgent))
            return BadRequest("Invalid edge client.");

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
        byte[] buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;
        do
        {
            try
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("WebSocket: " + message);

                    JToken? payload = JsonConvert.DeserializeObject<JToken>(message);
                    if (payload == null)
                        return;

                    DevSpaceShared.WebSocket.EventType EventType = payload["Type"]!.ToObject<DevSpaceShared.WebSocket.EventType>();

                    Logger.LogMessage("WebSocket", "Event: " + EventType.ToString(), LogSeverity.Info);

                    try
                    {
                        switch (EventType)
                        {
                            case EventType.TaskResponse:
                                {
                                    IWebSocketResponse<dynamic> @event = payload.ToObject<IWebSocketResponse<dynamic>>()!;
                                    if (edgeAgent.TaskCollection.TryGetValue(@event.TaskId, out TaskCompletionSource<JToken>? task))
                                    {
                                        Logger.LogMessage("WebSocket", "Got Response: " + @event.TaskId, LogSeverity.Info);
                                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(payload, Formatting.Indented));
                                        if (@event.IsSuccess)
                                            task.SetResult(payload["Data"]);
                                        else
                                            task.SetCanceled();
                                    }
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
            catch { }
        } while (!cancellationToken.IsCancellationRequested);
    }
}
