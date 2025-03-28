using System.Timers;

namespace DevSpaceWeb.Services;

public class BackgroundTasks
{
    public System.Timers.Timer timer;
    public BackgroundTasks()
    {
        if (!Program.IsDevMode)
        {
            Logger.LogMessage("Starting health check service", LogSeverity.Debug);
            timer = new System.Timers.Timer(new TimeSpan(1, 0, 0));
            timer.Elapsed += new ElapsedEventHandler(RunChecks);
            timer.Start();
        }
    }

    public async void RunChecks(object? sender, ElapsedEventArgs e)
    {
        // Version check
        VersionCheck? Version = null;
        try
        {
            string Test = await Program.Http.GetStringAsync("https://devspacesmtp.fluxpoint.dev/version");
            Version = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionCheck>(Test);
        }
        catch { }
        if (Version != null)
        {
            if (Program.LatestWebVersion != Version.WebVersion)
                Program.LatestWebVersion = Version.WebVersion;

            if (Program.LatestAgentVersion != Version.AgentVersion)
                Program.LatestAgentVersion = Version.AgentVersion;
        }
    }
}
public class VersionCheck
{
    public string WebVersion { get; set; }
    public string AgentVersion { get; set; }
}