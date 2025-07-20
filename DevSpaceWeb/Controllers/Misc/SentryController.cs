using DevSpaceShared;
using DevSpaceWeb.Data.ErrorLogs;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DevSpaceWeb.Controllers.Misc;

public class SentryController : Controller
{
    [HttpPost("/sentry/api/{projectId}/envelope")]
    [HttpPost("/sentry/api/{projectId}/store")]
    public async Task<IActionResult> Index([FromRoute] string projectId)
    {
        if (string.IsNullOrEmpty(projectId) || !ObjectId.TryParse(projectId, out ObjectId id) || !_DB.Projects.Cache.TryGetValue(id, out Data.Projects.ProjectData? project))
            return BadRequest();


        string? Auth = Request.Headers["X-Sentry-Auth"];
        string[] Split = Auth.Split(',');
        string? Key = Split.FirstOrDefault(x => x.StartsWith("sentry_key="));
        string body = "";

        if (string.IsNullOrEmpty(Key))
            return BadRequest();

        Key = Key.Replace("sentry_key=", "");

        if (Key != project.GetDecryptedLogKey())
            return BadRequest();

        Console.WriteLine("Got Sentry Request");

        try
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest("Request body does not have json content.");
        }

        string[] JsonValues = body.SplitNewlines(true);

        string? Data = JsonValues.LastOrDefault();
        SentryEvent? Event = SentryEvent.FromJson(JsonDocument.Parse(Data).RootElement);

        string? Hash = null;

        string? MessageTitle = null;
        string? Message = null;

        if (Event.SentryExceptions != null && Event.SentryExceptions.Any())
        {
            foreach (Sentry.Protocol.SentryException i in Event.SentryExceptions)
            {
                if (string.IsNullOrEmpty(MessageTitle))
                {
                    Hash += $"{i.Type}:{i.Value}";
                    MessageTitle = i.Type;
                    Message = i.Value;
                    break;
                }
            }
        }
        else if (Event.Message != null)
        {
            MessageTitle = "Message";
            Message = Event.Message.Formatted ?? Event.Message.Message;
            Hash = "Message:" + Event.Message.Message;
        }

        if (!string.IsNullOrEmpty(Hash))
            Hash = GetHashString(Hash);

        if (string.IsNullOrEmpty(Hash) || !Event.Level.HasValue || string.IsNullOrEmpty(Message) || string.IsNullOrEmpty(MessageTitle))
            return BadRequest();

        await project.LogLock.WaitAsync();
        try
        {
            FilterDefinition<LogData>[] Filters = new FilterDefinition<LogData>[]
            {
                new FilterDefinitionBuilder<LogData>().Eq(x => x.TeamId, project.TeamId),
                new FilterDefinitionBuilder<LogData>().Eq(x => x.ProjectId, project.Id),
                new FilterDefinitionBuilder<LogData>().Eq(x => x.Hash, Hash)
            };
            FilterDefinition<LogData> filter = new FilterDefinitionBuilder<LogData>().And(Filters);
            LogData? FoundData = _DB.Logs.Find(filter).FirstOrDefault();
            DateTime CurrentDate = DateTime.UtcNow;
            if (FoundData == null)
            {
                ulong IssueNumber = 1;
                if (project.CurrentIssueNumber == ulong.MaxValue)
                    IssueNumber = 1;
                else
                    IssueNumber = project.CurrentIssueNumber + 1;

                FoundData = new LogData
                {
                    Hash = Hash,
                    IssueNumber = IssueNumber,
                    LogType = GetType(Event.Level.Value),
                    Message = Message,
                    MessageTitle = MessageTitle,
                    ProjectId = project.Id,
                    TeamId = project.TeamId,
                    EventsCount = 1,
                    CreatedAt = CurrentDate,
                    LastSeenAt = CurrentDate
                };
                await _DB.Logs.CreateAsync(FoundData);
                await project.UpdateAsync(new UpdateDefinitionBuilder<ProjectData>().Set(x => x.CurrentIssueNumber, IssueNumber));
            }
            else
            {
                await FoundData.UpdateAsync(new UpdateDefinitionBuilder<LogData>().Set(x => x.LastSeenAt, CurrentDate));

                if (FoundData.Status == LogStatus.Resolved)
                    await FoundData.UpdateAsync(new UpdateDefinitionBuilder<LogData>().Set(x => x.Status, LogStatus.Open).Push(x => x.Activity, new LogActivity
                    {
                        ActivityType = LogActivityType.Opened
                    }));

                if (FoundData.EventsCount != ulong.MaxValue)
                    await FoundData.UpdateAsync(new UpdateDefinitionBuilder<LogData>().Set(x => x.EventsCount, FoundData.EventsCount + 1));
            }

            if (FoundData == null)
                return BadRequest();

            _ = _DB.LogsEvents.CreateAsync(new LogEventData
            {
                LogId = FoundData.Id,
                Json = Data,
                CreatedAt = CurrentDate,
            });
        }
        finally
        {
            project.LogLock.Release();
        }
        return Ok();
    }

    public LogType GetType(SentryLevel level)
    {
        switch (level)
        {
            case SentryLevel.Debug:
                return LogType.Debug;
            case SentryLevel.Fatal:
                return LogType.Fatal;
            case SentryLevel.Info:
                return LogType.Info;
            case SentryLevel.Warning:
                return LogType.Warn;
        }

        return LogType.Error;
    }

    public static byte[] GetHash(string inputString)
    {
        return MD5.HashData(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}
