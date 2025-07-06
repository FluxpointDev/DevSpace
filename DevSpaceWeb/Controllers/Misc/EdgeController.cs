using DevSpaceShared;
using DevSpaceShared.Agent;
using DevSpaceShared.WebSocket;
using DevSpaceWeb.Agents;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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
            {
                try
                {
                    edgeAgent.WebSocket.Dispose();
                }
                catch { }
            }

            edgeAgent.WebSocket = webSocket;

            Console.WriteLine("Edge connection handle");

            if (!server.IsAgentSetupComplete)
            {
                await server.UpdateAsync(new UpdateDefinitionBuilder<ServerData>().Set(x => x.IsAgentSetupComplete, true), () =>
                {
                    server.IsAgentSetupComplete = true;
                });
            }

            await Handle(webSocket, edgeAgent, cancellationToken);

            return Ok();
        }
        else
        {
            return Ok();
        }
    }

    [HttpPost("/edge/onboard")]
    public async Task<IActionResult> EdgeOnboard([FromBody] AgentOnboardCreate? data = null)
    {
        if (data == null)
            return BadRequest("Invalid onboard data.");

        if (string.IsNullOrEmpty(data.EdgeTeam))
            return BadRequest("Edge team is missing.");

        if (string.IsNullOrEmpty(data.EdgeOnboardKey))
            return BadRequest("Edge onboard key missing");

        if (!ObjectId.TryParse(data.EdgeTeam, out ObjectId teamId) || !_DB.Teams.Cache.TryGetValue(teamId, out TeamData? team))
            return BadRequest("Edge team is invalid.");

        if (data.EdgeOnboardKey != team.EdgeOnboardKey)
            return BadRequest("Edge onboard key is invalid.");

        if (!_DB.Users.TryGetValue(team.OwnerId, out PartialUserData? partialUser))
            return BadRequest("Failed to get team owner.");

        TeamMemberData? Owner = team.GetMember(partialUser);
        if (Owner == null)
            return BadRequest("Failed to get team owner.");

        ServerData Server = new ServerData
        {
            Name = data.ServerName,
            OwnerId = team.OwnerId,
            IsAgentSetupComplete = true,
            TeamId = team.Id,
            AgentId = null,
            AgentIp = null!,
            AgentKey = _Data.GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", ""),
            AgentType = ServerAgentType.Edge
        };

        try
        {
            await _DB.Servers.CreateAsync(Server);
        }
        catch
        {
            return BadRequest("Failed to create server.");
        }

        Server.StartWebSocket();

        _DB.Servers.Cache.TryAdd(Server.Id, Server);

        _ = _DB.AuditLogs.CreateAsync(new AuditLog(Owner, AuditLogCategoryType.Resource, AuditLogEventType.ServerOnboard)
        .SetTarget(Server)
        .SetFlags(AuditLogFlag.Automatic | AuditLogFlag.Sensitive)
        .AddProperty("Vanity URL", "")
        .AddProperty("Agent Type", "Edge"));

        return Ok(new AgentOnboardResponse
        {
            EdgeId = Server.Id.ToString(),
            EdgeKey = Server.AgentKey
        });
    }

    private static async Task Handle(WebSocket webSocket, EdgeAgent edgeAgent, CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
                    WebSocketReceiveResult? result = null;
                    do
                    {
                        try
                        {
                            result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        catch { }
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string? message = null;
                        using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            message = await reader.ReadToEndAsync();
                        }

                        JsonDocument? payload = JsonDocument.Parse(message, new JsonDocumentOptions
                        {
                            AllowTrailingCommas = true,
                        });
                        if (payload == null)
                            return;

                        try
                        {
                            EventType EventType = (EventType)payload.RootElement.GetProperty("Type").GetInt32();

                            Logger.LogMessage("WebSocket", "Edge Event: " + EventType.ToString(), LogSeverity.Info);


                            switch (EventType)
                            {
                                case EventType.TaskResponse:
                                    {
                                        IWebSocketResponse<dynamic> @event = payload.Deserialize<IWebSocketResponse<dynamic>>(AgentJsonOptions.Options)!;
                                        if (edgeAgent.TaskCollection.TryGetValue(@event.TaskId, out TaskCompletionSource<JsonElement>? task))
                                        {
                                            Logger.LogMessage("WebSocket", "Edge Got Response: " + @event.TaskId, LogSeverity.Info);
                                            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(payload, Formatting.Indented));
                                            if (@event.IsSuccess)
                                                task.SetResult(payload.RootElement.GetProperty("Data"));
                                            else
                                                task.SetCanceled();
                                        }
                                    }
                                    break;
                                case EventType.GetAgentStats:
                                    {
                                        AgentStatsResponse? Data = payload.Deserialize<AgentStatsResponse>(AgentJsonOptions.Options);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        } while (!cancellationToken.IsCancellationRequested);
        Console.WriteLine("Edge Handle Removed");
    }
}
