using DevSpaceAgent.Data;
using DevSpaceAgent.Server;
using DevSpaceShared;
using DevSpaceShared.Agent;
using DevSpaceShared.Events.Docker;
using DevSpaceShared.Responses;
using DevSpaceShared.WebSocket;
using Docker.DotNet;
using System.Text;
using System.Text.Json;

namespace DevSpaceAgent.Client;

public static class ServerEventHandler
{
    public static async Task RecieveAsync(ISession ws, byte[] buffer, long offset, long size)
    {
        DateTime Now = DateTime.UtcNow;
        string json = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        JsonDocument? payload = JsonDocument.Parse(json, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
        });

        TimeSpan Current = DateTime.UtcNow - Now;
        Console.WriteLine("Recieve Time: " + Current.TotalMilliseconds);
        if (payload == null)
            return;

        EventType EventType = (EventType)payload.RootElement.GetProperty("Type").GetInt32();

        Console.WriteLine("Event: " + EventType.ToString());

        try
        {
            switch (EventType)
            {
                case EventType.Pong:
                    {
                        Console.WriteLine("PONG");
                    }
                    break;
                case EventType.GetAgentOptions:
                    {
                        IWebSocketTask? task = payload.Deserialize<IWebSocketTask>(AgentJsonOptions.Options);
                        if (task == null)
                            return;

                        await ws.RespondAsync(task.TaskId, new SocketResponse
                        {
                            IsSuccess = true,
                            Data = _Data.Config.Options
                        });
                    }
                    break;
                case EventType.UpdateAgentOptions:
                    {
                        AgentOptionsUpdate? data = payload.Deserialize<AgentOptionsUpdate>(AgentJsonOptions.Options);
                        if (data == null)
                            return;

                        _Data.Config.Options.Update(data);
                        _Data.Config.Save();

                        await ws.RespondAsync(data.TaskId, new SocketResponse<object?>()
                        {
                            IsSuccess = true
                        });
                    }
                    break;
                case EventType.Docker:
                    {
                        Console.WriteLine("Got: " + json);
                        DockerEvent? @event = payload.Deserialize<DockerEvent>(AgentJsonOptions.Options);
                        if (@event == null)
                            return;

                        SocketResponse<object?> response = new SocketResponse<object?>();
                        if (Program.DockerFailed || Program.DockerClient == null)
                        {
                            response.DockerError = DockerError.NotInstalled;
                        }
                        else
                        {
                            try
                            {
                                response.Data = await DockerHandler.Run(@event);
                                response.IsSuccess = true;
                            }
                            catch (DockerApiException de)
                            {
                                Console.WriteLine(de);
                                response.Message = de.Message;
                                response.DockerError = DockerError.Exception;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                response.Message = ex.Message;
                                response.Error = ClientError.Exception;
                            }
                        }

                        bool NoResponse = true;
                        switch (@event.DockerType)
                        {
                            case DockerEventType.ListContainers:
                            case DockerEventType.ListImages:
                            case DockerEventType.ListNetworks:
                            case DockerEventType.ListPlugins:
                            case DockerEventType.ListStacks:
                            case DockerEventType.ListVolumes:
                                NoResponse = true;
                                break;
                        }

                        await ws.RespondAsync(@event.TaskId, response, NoResponse);
                    }

                    break;
                    //case EventType.SystemInfo:
                    //    {
                    //        IWebSocketTaskEvent @event = payload.ToObject<IWebSocketTaskEvent>()!;
                    //        DevSpaceShared.Responses.SystemInfoResponse? info = null;
                    //        try
                    //        {
                    //            TimeSpan runtime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                    //            List<string> Times = new List<string>();
                    //            if (runtime.Days != 0)
                    //                Times.Add($"{runtime.Days} Days");
                    //            if (runtime.Hours != 0)
                    //                Times.Add($"{runtime.Hours} Hours");
                    //            if (runtime.Minutes != 0)
                    //                Times.Add($"{runtime.Minutes} Minutes");
                    //            Times.Add($"{runtime.Seconds} Seconds");
                    //            info = new DevSpaceShared.Responses.SystemInfoResponse
                    //            {
                    //                Host = new HostInfo
                    //                {
                    //                    MachineName = Environment.MachineName,
                    //                    SystemVersion = Environment.OSVersion.ToString(),
                    //                    Uptime = string.Join(" ", Times)
                    //                },
                    //                Process = new ProcessInfo
                    //                {
                    //                    DotnetVersion = Environment.Version.ToString(),
                    //                    ProcessId = Environment.ProcessId,
                    //                    ProcessPath = Environment.ProcessPath?.Replace("/DevSpaceAgent.dll", ""),
                    //                }
                    //            };

                    //            foreach (object? i in Environment.GetEnvironmentVariables())
                    //            {
                    //                info.Process.EnvironmentVariables.Add(i.ToString());
                    //            }

                    //            Command cmd = Cli.Wrap("free").WithArguments("-m");
                    //            BufferedCommandResult res = await cmd.ExecuteBufferedAsync();
                    //            if (res.IsSuccess)
                    //            {
                    //                string[] Response = res.StandardOutput.SplitNewlines();
                    //                string RamLine = Response[1];
                    //                string SwapLine = Response[2];
                    //                string[] RamProps = RamLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    //                string[] SwapProps = SwapLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    //                info.Host.RamTotal = Math.Round(double.Parse(RamProps[1]) / 1024, 0);
                    //                info.Host.RamUsed = double.Parse(RamProps[2]);
                    //                info.Host.RamFree = double.Parse(RamProps[3]);

                    //                info.Host.SwapTotal = double.Parse(SwapProps[1]);
                    //                info.Host.SwapUsed = double.Parse(SwapProps[2]);
                    //                info.Host.SwapFree = double.Parse(SwapProps[3]);
                    //            }

                    //            cmd = Cli.Wrap("lsb_release").WithArguments("-a");
                    //            res = await cmd.ExecuteBufferedAsync();
                    //            if (res.IsSuccess)
                    //            {
                    //                string[] Response = res.StandardOutput.SplitNewlines();
                    //                info.Host.Release = Response.FirstOrDefault(x => x.StartsWith("Description:"))?.Split(":", StringSplitOptions.TrimEntries)[1];
                    //            }

                    //            cmd = Cli.Wrap("lscpu").WithArguments("--json");
                    //            res = await cmd.ExecuteBufferedAsync();
                    //            if (res.IsSuccess)
                    //            {
                    //                LinuxJsonCpu? cpu = Newtonsoft.Json.JsonConvert.DeserializeObject<LinuxJsonCpu>(res.StandardOutput);
                    //                if (cpu != null)
                    //                {
                    //                    int Count = 0;
                    //                    int ChildCount = 0;
                    //                    int ExtraChildCount = 0;
                    //                    foreach (LinuxJson i in cpu.lscpu)
                    //                    {
                    //                        switch (i.field)
                    //                        {
                    //                            case "CPU(s):":
                    //                                info.Host.Cpu.CpuCount = int.Parse(i.data);
                    //                                break;
                    //                            case "Vendor ID:":
                    //                                info.Host.Cpu.VendorId = i.data;
                    //                                break;
                    //                            case "Model name:":
                    //                                info.Host.Cpu.ModelName = i.data;
                    //                                break;
                    //                            case "CPU family:":
                    //                                info.Host.Cpu.CpuFamily = int.Parse(i.data);
                    //                                break;
                    //                            case "Model:":
                    //                                info.Host.Cpu.Model = int.Parse(i.data);
                    //                                break;
                    //                            case "Thread(s) per core:":
                    //                                info.Host.Cpu.ThreadsPerCore = int.Parse(i.data);
                    //                                break;
                    //                            case "Core(s) per socket:":
                    //                                info.Host.Cpu.CoresPerSocket = int.Parse(i.data);
                    //                                break;
                    //                            case "Socket(s):":
                    //                                info.Host.Cpu.Sockets = int.Parse(i.data);
                    //                                break;
                    //                            case "Flags:":
                    //                                info.Host.Cpu.Flags = i.data;
                    //                                break;
                    //                            case "Virtualization:":
                    //                                info.Host.Cpu.VirtualizationType = i.data;
                    //                                break;
                    //                            case "Hypervisor vendor:":
                    //                                info.Host.Cpu.VirtualizationHypervisor = i.data;
                    //                                break;
                    //                            case "Virtualization type:":
                    //                                info.Host.Cpu.VirtualizationMode = i.data;
                    //                                break;
                    //                        }
                    //                        if (i.field.StartsWith("Vulnerability "))
                    //                            info.Host.Cpu.Vulnerabilities.Add(i.field.Replace("Vulnerability ", ""), i.data);
                    //                        Count += 1;
                    //                        ChildCount = 0;
                    //                        ExtraChildCount = 0;
                    //                    }
                    //                }
                    //            }

                    //            await ws.RespondAsync(@event.TaskId, info);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Console.WriteLine(ex);
                    //            await ws.RespondFailAsync(@event.TaskId);
                    //        }
                    //    }
                    //    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket Event {EventType.ToString()} Error ");
            Console.WriteLine(ex);
        }
    }
}
