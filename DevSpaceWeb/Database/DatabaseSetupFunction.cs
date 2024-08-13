using CliWrap;
using CliWrap.Buffered;

namespace DevSpaceWeb.Database
{
    public static class DatabaseSetupFunction
    {
        public static async Task<DatabaseSetupErrorType> Run()
        {
            try
            {
                var cmd = Cli.Wrap("cat").WithArguments("/etc/lsb-release");

                BufferedCommandResult? res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                    return DatabaseSetupErrorType.InvalidOS;

                bool IsSupported = false;
                bool UbuntuJammy22 = true;
                string[] lines = res.StandardOutput.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                string Dist = lines[0].Split('=').Last();
                string Release = lines[1].Split('=').Last();

                switch (Dist)
                {
                    case "Ubuntu":
                        {
                            switch (Release.Split('.').First())
                            {
                                case "20":
                                case "22":
                                case "24":
                                    IsSupported = true;
                                    if (Release.Split('.').First() == "20")
                                        UbuntuJammy22 = false;
                                    break;
                            }
                        }
                        break;
                    case "Debian":
                        {
                            switch (Release)
                            {
                                case "11":
                                case "12":
                                    //IsSupported = true;
                                    break;
                            }
                        }
                        break;
                }

                if (!IsSupported)
                    return DatabaseSetupErrorType.UnsupportedOS;

                cmd = Cli.Wrap("sudo").WithArguments("-v");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedSudo;
                }

                if (res.StandardOutput.Contains("may not run sudo"))
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.NoSudoAccess;
                }


                cmd = Cli.Wrap("sudo").WithArguments("apt install -y gnupg curl");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedSudo;
                }


                try
                {
                    var Res = await new HttpClient().GetStringAsync("https://www.mongodb.org/static/pgp/server-7.0.asc");
                    File.WriteAllText(Program.CurrentDirectory + "Data/mongodb.asc", Res);
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(Program.CurrentDirectory + "Data/mongodb.asc");
                    }
                    catch { }
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedAptInstall;
                }

                cmd = Cli.Wrap("gpg").WithWorkingDirectory(Program.CurrentDirectory + "Data").WithArguments("--dearmor --yes mongodb.asc");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    try
                    {
                        File.Delete(Program.CurrentDirectory + "Data/mongodb.asc");
                    }
                    catch { }

                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedAptInstall;
                }

                try
                {
                    File.Delete(Program.CurrentDirectory + "Data/mongodb.asc");
                }
                catch { }

                if (!File.Exists("/usr/share/keyrings/mongodb-server-7.0.gpg"))
                {
                    try
                    {
                        File.Move(Program.CurrentDirectory + "Data/mongodb.asc.gpg", "/usr/share/keyrings/mongodb-server-7.0.gpg");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("--- Database Setup Error ---");
                        Console.WriteLine(ex);
                        Console.WriteLine("--- --- --- ---- --- --- ---");
                        return DatabaseSetupErrorType.FailedAptInstall;
                    }
                }

                if (UbuntuJammy22)
                    cmd = Cli.Wrap("echo").WithArguments("\"deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-7.0.gpg ] https://repo.mongodb.org/apt/ubuntu jammy/mongodb-org/7.0 multiverse\" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list");
                else
                    cmd = Cli.Wrap("echo").WithArguments("\"deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-7.0.gpg ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/7.0 multiverse\" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedAptInstall;
                }


                cmd = Cli.Wrap("sudo").WithArguments("apt update");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedAptInstall;
                }


                cmd = Cli.Wrap("sudo").WithArguments("apt install -y mongodb-org");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedAptInstall;
                }


                cmd = Cli.Wrap("ps").WithArguments("--no-headers -o comm 1");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Console.WriteLine("--- Database Setup Error ---");
                    Console.WriteLine(res.StandardError);
                    Console.WriteLine("--- --- --- ---- --- --- ---");
                    return DatabaseSetupErrorType.FailedSystemProcess;
                }

                Console.WriteLine("Type: " + res.StandardOutput);

                switch (res.StandardOutput.Trim())
                {
                    case "systemd":
                        {
                            cmd = Cli.Wrap("sudo").WithArguments("systemctl start mongod");

                            res = await cmd.ExecuteBufferedAsync();
                            if (!res.IsSuccess)
                            {
                                Console.WriteLine("--- Database Setup Start ---");
                                Console.WriteLine(res.StandardError);
                                Console.WriteLine("--- --- --- ---- --- --- ---");

                                if (res.StandardError.Contains("Failed to start"))
                                {
                                    cmd = Cli.Wrap("sudo").WithArguments("systemctl daemon-reload");

                                    res = await cmd.ExecuteBufferedAsync();
                                    if (!res.IsSuccess)
                                    {
                                        Console.WriteLine("--- Database Setup Error ---");
                                        Console.WriteLine(res.StandardError);
                                        Console.WriteLine("--- --- --- ---- --- --- ---");
                                        return DatabaseSetupErrorType.FailedSystemProcess;
                                    }


                                    cmd = Cli.Wrap("sudo").WithArguments("systemctl start mongod");

                                    res = await cmd.ExecuteBufferedAsync();
                                    if (!res.IsSuccess)
                                    {
                                        Console.WriteLine("--- Database Setup Error ---");
                                        Console.WriteLine(res.StandardError);
                                        Console.WriteLine("--- --- --- ---- --- --- ---");
                                        return DatabaseSetupErrorType.FailedSystemProcess;
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("--- Database Setup Error ---");
                                    Console.WriteLine(res.StandardError);
                                    Console.WriteLine("--- --- --- ---- --- --- ---");
                                    return DatabaseSetupErrorType.FailedSystemProcess;
                                }


                                cmd = Cli.Wrap("sudo").WithArguments("systemctl enable mongod");

                                res = await cmd.ExecuteBufferedAsync();
                                if (!res.IsSuccess)
                                {
                                    Console.WriteLine("--- Database Setup Error ---");
                                    Console.WriteLine(res.StandardError);
                                    Console.WriteLine("--- --- --- ---- --- --- ---");
                                    return DatabaseSetupErrorType.FailedSystemProcess;
                                }

                            }
                        }
                        break;
                    case "init":
                        {
                            cmd = Cli.Wrap("sudo").WithArguments("service mongod start");

                            res = await cmd.ExecuteBufferedAsync();
                            if (!res.IsSuccess)
                            {
                                Console.WriteLine("--- Database Setup Error ---");
                                Console.WriteLine(res.StandardError);
                                Console.WriteLine("--- --- --- ---- --- --- ---");
                                return DatabaseSetupErrorType.FailedSystemProcess;
                            }

                        }
                        break;
                    default:
                        return DatabaseSetupErrorType.InvalidSystemProcess;
                }



                return DatabaseSetupErrorType.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return DatabaseSetupErrorType.Exception;
            }
        }
    }

    public enum DatabaseSetupErrorType
    {
        Exception, Success, InvalidOS, UnsupportedOS, FailedSudo, NoSudoAccess,
        FailedAptInstall, FailedSystemProcess, InvalidSystemProcess
    }
}
