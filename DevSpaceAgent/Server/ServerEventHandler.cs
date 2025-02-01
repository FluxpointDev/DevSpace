using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using DevSpaceAgent.Data;
using DevSpaceAgent.Server;
using DevSpaceShared;
using DevSpaceShared.Events.Docker;
using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DevSpaceAgent.Client;

public static class ServerEventHandler
{
    public static async Task RecieveAsync(AgentWebSocket ws, byte[] buffer, long offset, long size)
    {
        string json = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        JToken payload = JsonConvert.DeserializeObject<JToken>(json)!;

        if (payload == null)
            return;

        EventType EventType = payload!["type"]!.ToObject<EventType>();

        Console.WriteLine("Event: " + EventType.ToString());

        try
        {
            switch (EventType)
            {
                case EventType.ValidateCert:
                    {
                        ValidateCertEvent @event = payload.ToObject<ValidateCertEvent>()!;
                        Console.WriteLine("Server Validate Cert: " + @event.CertHash);
                        if (Program.Certificate.GetCertHashString() == @event.CertHash)
                        {
                            Console.WriteLine("Validated");
                            ws.IsCertValid = true;
                            ws.SendJsonAsync(new ValidateCertEvent { CertHash = @event.CertHash });
                        }
                    }
                    break;
                case EventType.Pong:
                    {
                        Console.WriteLine("PONG");
                    }
                    break;
                case EventType.Command:
                    {
                        DevSpaceShared.WebSocket.CommandEvent @event = payload.ToObject<DevSpaceShared.WebSocket.CommandEvent>()!;
                        string[] Split = @event.Command.Trim().Split(' ');
                        if (string.IsNullOrEmpty(Split[0]))
                            return;

                        Command cmd = Cli.Wrap(Split[0]);
                        if (Split.Length > 1)
                            cmd = cmd.WithArguments(string.Join(' ', Split.Skip(1)));

                        _ = cmd.ExecuteAsync();
                    }
                    break;
                case EventType.Docker:
                    {
                        DockerEvent @event = payload.ToObject<DockerEvent>()!;
                        DockerResponse<object?> response = new DockerResponse<object?>();
                        if (Program.DockerFailed)
                        {
                            response.Error = DockerError.NotInstalled;
                        }
                        else
                        {
                            try
                            {
                                response.Data = await DockerHandler.Run(@event);
                            }
                            catch (Exception ex)
                            {
                                response.Data = ex.Message;
                                response.Error = DockerError.Failed;
                            }
                        }

                        await ws.RespondAsync(@event.TaskId, response);
                    }

                    break;
                case EventType.CommandWait:
                    {
                        DevSpaceShared.WebSocket.CommandEvent @event = payload.ToObject<DevSpaceShared.WebSocket.CommandEvent>()!;

                        string[] Split = @event.Command.Trim().Split(' ');
                        if (string.IsNullOrEmpty(Split[0]))
                            return;

                        Command cmd = Cli.Wrap(Split[0]).WithWorkingDirectory(Program.CurrentDirectory + "Data");
                        if (Split.Length > 1)
                        {
                            cmd = cmd.WithArguments(string.Join(' ', Split.Skip(1)));
                        }

                        Console.WriteLine("Command: " + cmd.Arguments);

                        Console.WriteLine("Send Response: " + @event.TaskId);
                        BufferedCommandResult? res = await cmd.ExecuteBufferedAsync();
                        Console.WriteLine("Send Response 2");
                        if (res.IsSuccess)
                            await ws.RespondAsync(@event.TaskId, new CommandResponse() { Output = res.StandardOutput });
                        else
                            await ws.RespondFailAsync(@event.TaskId);
                    }
                    break;
                case EventType.CommandStream:
                    {
                        CommandStreamEvent @event = payload.ToObject<CommandStreamEvent>()!;
                        string[] Split = @event.Command.Trim().Split(' ');
                        if (string.IsNullOrEmpty(Split[0]))
                            return;

                        Command cmd = Cli.Wrap(Split[0]);
                        if (Split.Length > 1)
                            cmd = cmd.WithArguments(string.Join(' ', Split.Skip(1)));

                        Console.WriteLine("Stream Start");
                        await foreach (CliWrap.EventStream.CommandEvent cmdEvent in cmd.ListenAsync())
                        {
                            switch (cmdEvent)
                            {
                                case StartedCommandEvent started:
                                    Console.WriteLine($"Process started; ID: {started.ProcessId}");
                                    break;
                                case StandardOutputCommandEvent stdOut:
                                    ws.SendJsonAsync(new CommandStreamEvent
                                    {
                                        Stream = new List<string>
                                        {
                                            stdOut.Text
                                        }
                                    });
                                    Console.WriteLine($"Out> {stdOut.Text}");
                                    break;
                                case StandardErrorCommandEvent stdErr:
                                    Console.WriteLine($"Err> {stdErr.Text}");
                                    ws.SendJsonAsync(new CommandStreamEvent
                                    {
                                        Stream = new List<string>
                                        {
                                            stdErr.Text
                                        }
                                    });
                                    break;
                                case ExitedCommandEvent exited:
                                    Console.WriteLine($"Process exited; Code: {exited.ExitCode}");
                                    break;
                            }
                        }
                        Console.WriteLine("Stream Done");
                    }
                    break;
                case EventType.FirewallInfo:
                    {
                        IWebSocketTaskEvent @event = payload.ToObject<IWebSocketTaskEvent>()!;

                        Command cmd = Cli.Wrap("sudo").WithArguments("ufw status verbose");
                        BufferedCommandResult? res = await cmd.ExecuteBufferedAsync();

                        string[] lines = res.StandardOutput.SplitNewlines();

                        FirewallResponse response = new FirewallResponse();
                        if (lines[0] != "Status: active")
                        {
                            // Firewall not enabled
                        }
                        else
                        {
                            string Logging = string.Empty;
                            string Default = string.Empty;
                            string NewProfiles = string.Empty;
                            List<string> Rules = new List<string>();
                            bool RulesAfter = false;
                            foreach (string i in lines)
                            {
                                Console.WriteLine("Line: " + i);
                                if (i.StartsWith("Logging: "))
                                {
                                    Logging = i.Replace("Logging: ", "");
                                }
                                else if (i.StartsWith("Default: "))
                                {
                                    Default = i.Replace("Default: ", "");
                                }
                                else if (i.StartsWith("New profiles: "))
                                {
                                    NewProfiles = i.Replace("New profiles: ", "");
                                }
                                else if (i.StartsWith("--"))
                                {
                                    RulesAfter = true;
                                }
                                else if (RulesAfter)
                                {
                                    Rules.Add(i);
                                }
                            }

                            response.IsLoggingEnabled = Logging.StartsWith("on");
                            Logging = Logging.Replace("on", "").Replace("off", "").Replace(" (", "").Replace(")", "");
                            response.LoggingMode = Logging;

                            string[] Defaults = Default.Split(", ");
                            foreach (string i in Defaults)
                            {
                                if (i.EndsWith("(incoming)"))
                                    response.Default.Incoming = i.Replace(" (incoming)", "");
                                else if (i.EndsWith("(outgoing)"))
                                    response.Default.Outgoing = i.Replace(" (outgoing)", "");
                                else if (i.EndsWith("(routed)"))
                                    response.Default.Routed = i.Replace(" (routed)", "");
                            }

                            foreach (string i in Rules)
                            {
                                string[] rulesLines = i.Split("  ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                Console.WriteLine("Rule: " + string.Join(" | ", rulesLines));
                                string To = rulesLines[0].Trim();
                                string Type = rulesLines[1].Trim();
                                string From = rulesLines[2].Trim();
                                string Comment = string.Join("  ", rulesLines.Skip(3)).Trim();
                                if (Comment.Length > 3)
                                    Comment = Comment.Substring(2);
                                response.Rules.Add(new FirewallRuleResponse
                                {
                                    Action = Type,
                                    To = To,
                                    From = From,
                                    Comment = Comment
                                });
                            }
                        }


                        if (res.IsSuccess)
                        {
                            await ws.RespondAsync(@event.TaskId, response);
                        }
                        else
                            await ws.RespondFailAsync(@event.TaskId);


                    }
                    break;
                case EventType.SystemInfo:
                    {
                        IWebSocketTaskEvent @event = payload.ToObject<IWebSocketTaskEvent>()!;
                        DevSpaceShared.Responses.SystemInfoResponse? info = null;
                        try
                        {
                            TimeSpan runtime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                            List<string> Times = new List<string>();
                            if (runtime.Days != 0)
                                Times.Add($"{runtime.Days} Days");
                            if (runtime.Hours != 0)
                                Times.Add($"{runtime.Hours} Hours");
                            if (runtime.Minutes != 0)
                                Times.Add($"{runtime.Minutes} Minutes");
                            Times.Add($"{runtime.Seconds} Seconds");
                            info = new DevSpaceShared.Responses.SystemInfoResponse
                            {
                                Host = new HostInfo
                                {
                                    MachineName = Environment.MachineName,
                                    SystemVersion = Environment.OSVersion.ToString(),
                                    Uptime = string.Join(" ", Times)
                                },
                                Process = new ProcessInfo
                                {
                                    DotnetVersion = Environment.Version.ToString(),
                                    ProcessId = Environment.ProcessId,
                                    ProcessPath = Environment.ProcessPath?.Replace("/DevSpaceAgent.dll", ""),
                                }
                            };

                            foreach (object? i in Environment.GetEnvironmentVariables())
                            {
                                info.Process.EnvironmentVariables.Add(i.ToString());
                            }

                            Command cmd = Cli.Wrap("free").WithArguments("-m");
                            BufferedCommandResult res = await cmd.ExecuteBufferedAsync();
                            if (res.IsSuccess)
                            {
                                string[] Response = res.StandardOutput.SplitNewlines();
                                string RamLine = Response[1];
                                string SwapLine = Response[2];
                                string[] RamProps = RamLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                string[] SwapProps = SwapLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                info.Host.RamTotal = Math.Round(double.Parse(RamProps[1]) / 1024, 0);
                                info.Host.RamUsed = double.Parse(RamProps[2]);
                                info.Host.RamFree = double.Parse(RamProps[3]);

                                info.Host.SwapTotal = double.Parse(SwapProps[1]);
                                info.Host.SwapUsed = double.Parse(SwapProps[2]);
                                info.Host.SwapFree = double.Parse(SwapProps[3]);
                            }

                            cmd = Cli.Wrap("lsb_release").WithArguments("-a");
                            res = await cmd.ExecuteBufferedAsync();
                            if (res.IsSuccess)
                            {
                                string[] Response = res.StandardOutput.SplitNewlines();
                                info.Host.Release = Response.FirstOrDefault(x => x.StartsWith("Description:"))?.Split(":", StringSplitOptions.TrimEntries)[1];
                            }

                            cmd = Cli.Wrap("lscpu").WithArguments("--json");
                            res = await cmd.ExecuteBufferedAsync();
                            if (res.IsSuccess)
                            {
                                LinuxJsonCpu? cpu = Newtonsoft.Json.JsonConvert.DeserializeObject<LinuxJsonCpu>(res.StandardOutput);
                                if (cpu != null)
                                {
                                    int Count = 0;
                                    int ChildCount = 0;
                                    int ExtraChildCount = 0;
                                    foreach (LinuxJson i in cpu.lscpu)
                                    {
                                        switch (i.field)
                                        {
                                            case "CPU(s):":
                                                info.Host.Cpu.CpuCount = int.Parse(i.data);
                                                break;
                                            case "Vendor ID:":
                                                info.Host.Cpu.VendorId = i.data;
                                                break;
                                            case "Model name:":
                                                info.Host.Cpu.ModelName = i.data;
                                                break;
                                            case "CPU family:":
                                                info.Host.Cpu.CpuFamily = int.Parse(i.data);
                                                break;
                                            case "Model:":
                                                info.Host.Cpu.Model = int.Parse(i.data);
                                                break;
                                            case "Thread(s) per core:":
                                                info.Host.Cpu.ThreadsPerCore = int.Parse(i.data);
                                                break;
                                            case "Core(s) per socket:":
                                                info.Host.Cpu.CoresPerSocket = int.Parse(i.data);
                                                break;
                                            case "Socket(s):":
                                                info.Host.Cpu.Sockets = int.Parse(i.data);
                                                break;
                                            case "Flags:":
                                                info.Host.Cpu.Flags = i.data;
                                                break;
                                            case "Virtualization:":
                                                info.Host.Cpu.VirtualizationType = i.data;
                                                break;
                                            case "Hypervisor vendor:":
                                                info.Host.Cpu.VirtualizationHypervisor = i.data;
                                                break;
                                            case "Virtualization type:":
                                                info.Host.Cpu.VirtualizationMode = i.data;
                                                break;
                                        }
                                        if (i.field.StartsWith("Vulnerability "))
                                            info.Host.Cpu.Vulnerabilities.Add(i.field.Replace("Vulnerability ", ""), i.data);
                                        Count += 1;
                                        ChildCount = 0;
                                        ExtraChildCount = 0;
                                    }
                                }
                            }

                            await ws.RespondAsync(@event.TaskId, info);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            await ws.RespondFailAsync(@event.TaskId);
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket Event {payload["type"].ToString()} Error ");
            Console.WriteLine(ex);
        }
    }
}
