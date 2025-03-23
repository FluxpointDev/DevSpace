using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using System.Text;
using System.Text.Json;
using System.Timers;

namespace DevSpaceWeb.Services;

public class DatabaseHealthCheck : IHealthCheck
{
    public DatabaseHealthCheck(HealthCheckService service)
    {
        HealthService = service;
    }

    public HealthCheckService HealthService;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        if (_DB.HasException)
            return HealthCheckResult.Degraded("Database failed to load data.");

        if (!_DB.IsConnected)
            return HealthCheckResult.Degraded("Database failed to connect during startup.");

        if (!HealthService.IsDatabaseOnline)
            return HealthCheckResult.Degraded("Database has gone down.");

        return HealthCheckResult.Healthy();
    }
}

public class EmailHealthCheck : IHealthCheck
{
    public EmailHealthCheck(HealthCheckService service)
    {
        HealthService = service;
    }

    public HealthCheckService HealthService;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        if (!HealthService.IsEmailOnline)
            return HealthCheckResult.Degraded("Email service is down.");

        if (HealthService.EmailDownCount == 3)
            return HealthCheckResult.Degraded("Email service is down.");

        if (HealthService.EmailFailCount == 3)
            return HealthCheckResult.Unhealthy("Email service is failing to send.");

        return HealthCheckResult.Healthy();
    }
}

public class HealthCheckService
{
    public System.Timers.Timer timer;
    public HealthCheckService()
    {
        if (!Program.IsDevMode)
        {
            Logger.LogMessage("Starting health check service", LogSeverity.Debug);
            timer = new System.Timers.Timer(new TimeSpan(0, 5, 0));
            timer.Elapsed += new ElapsedEventHandler(RunChecks);
            timer.Start();
        }
    }

    public bool IsDatabaseOnline = true;
    public bool IsEmailOnline = true;
    public int UnHealthyCount = 0;
    public int EmailFailCount = 0;
    public int EmailDownCount = 0;

    private async void RunChecks(object? sender, ElapsedEventArgs e)
    {
        Logger.LogMessage("Running health checks", LogSeverity.Debug);
        try
        {
            BsonDocument result = await _DB.Run.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            IsDatabaseOnline = true;
        }
        catch
        {
            IsDatabaseOnline = false;
        }

        if (_Data.Config.Email.Type == ConfigEmailType.FluxpointManaged)
        {
            HttpRequestMessage Message = new HttpRequestMessage(HttpMethod.Get, "https://devspacesmtp.fluxpoint.dev/test");
            Message.Headers.Add("Authorization", _Data.Config.Email.ManagedEmailToken);

            HttpResponseMessage Res = await Program.Http.SendAsync(Message);
            IsEmailOnline = Res.IsSuccessStatusCode;
        }

        if (IsDatabaseOnline && IsEmailOnline)
        {
            if (UnHealthyCount != 0)
                UnHealthyCount = 0;
        }
        else
        {
            if (UnHealthyCount != 3)
                UnHealthyCount += 1;
        }
    }

    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        if (healthReport.Status != HealthStatus.Healthy)
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}