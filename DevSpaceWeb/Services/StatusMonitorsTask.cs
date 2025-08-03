using DevSpaceWeb.Data.Status;
using DevSpaceWeb.Database;
using System.Net.NetworkInformation;
using System.Timers;

namespace DevSpaceWeb.Services;

public class StatusMonitorsTask
{
    public System.Timers.Timer timer = new System.Timers.Timer(new TimeSpan(0, 2, 0));

    public StatusMonitorsTask()
    {
        Logger.LogMessage("Starting status monitors service", LogSeverity.Debug);
        timer.Elapsed += new ElapsedEventHandler(RunChecks);
        timer.Start();
    }

    public async void RunChecks(object? sender, ElapsedEventArgs e)
    {
        timer.Stop();

        foreach (StatusMonitorData i in _DB.StatusMonitors.Cache.Values)
        {
            bool IsAlive = false;
            switch (i.MonitorType)
            {
                case StatusMonitorType.Ping:

                    using (Ping pinger = new Ping())
                    {
                        try
                        {
                            PingReply reply = pinger.Send(i.Source.Replace("https://", "", StringComparison.OrdinalIgnoreCase).Replace("http://", "", StringComparison.OrdinalIgnoreCase));
                            IsAlive = reply.Status == IPStatus.Success;
                        }
                        catch { }
                    }
                    break;
                case StatusMonitorType.Http:
                    {
                        HttpRequestMessage message = new HttpRequestMessage(i.Flag.HasFlag(StatusMonitorFlag.HttpGet) ? HttpMethod.Get : HttpMethod.Head, i.Source);
                        using (HttpResponseMessage req = await Program.Http.SendAsync(message))
                        {
                            IsAlive = req.StatusCode == System.Net.HttpStatusCode.OK;
                        }
                    }
                    break;
            }

            if (IsAlive)
            {
                if (i.State.IsDown || !i.State.Uptime.HasValue)
                    i.State.Uptime = DateTime.UtcNow;

                i.State.HasFailedPreviously = false;
                i.State.IsDown = false;
            }
            else
            {
                if (i.State.IsDown)
                    continue;

                if (i.State.HasFailedPreviously)
                {
                    i.State.Uptime = DateTime.UtcNow;
                    i.State.IsDown = true;
                }
                else
                    i.State.HasFailedPreviously = true;
            }
        }

        timer.Start();
    }
}
