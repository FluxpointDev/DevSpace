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
                Command cmd = Cli.Wrap("cat").WithArguments("/etc/lsb-release");

                BufferedCommandResult? res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                    return DatabaseSetupErrorType.InvalidOS;

                bool IsSupported = false;
                //bool UbuntuJammy22 = true;
                string[] lines = res.StandardOutput.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                string Dist = lines[0].Split('=').Last().ToLower();
                string Codename = lines[2].Split('=').Last().ToLower();
                //string Release = lines[1].Split('=').Last();

                string MongoVersion = "7.0";

                switch (Dist)
                {
                    case "ubuntu":
                        {
                            switch (Codename)
                            {
                                case "bionic":
                                case "xenial":
                                    IsSupported = true;
                                    break;
                                case "focal":
                                case "jammy":
                                    IsSupported = true;
                                    ////MongoVersion = "8.0";
                                    break;
                                case "noble":
                                    Codename = "jammy";
                                    IsSupported = true;
                                    ////MongoVersion = "8.0";
                                    break;
                            }
                        }
                        break;
                    case "debian":
                        {
                            switch (Codename)
                            {
                                case "bookworm":
                                    IsSupported = true;
                                    //MongoVersion = "8.0";
                                    break;
                                case "bullseye":
                                case "buster":
                                case "jessie":
                                case "stretch":
                                    IsSupported = true;
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
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedSudo;
                }

                if (res.StandardOutput.Contains("may not run sudo"))
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.NoSudoAccess;
                }


                cmd = Cli.Wrap("sudo").WithArguments("apt install -y gnupg curl");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedSudo;
                }


                try
                {
                    string Res = await new HttpClient().GetStringAsync("https://www.mongodb.org/static/pgp/server-" + MongoVersion + ".asc");
                    File.WriteAllText(Program.Directory.Cache.Path + "mongodb.asc", Res);
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(Program.Directory.Cache.Path + "mongodb.asc");
                    }
                    catch { }
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(ex.ToString(), LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedAptInstall;
                }

                cmd = Cli.Wrap("sudo").WithWorkingDirectory(Program.Directory.Cache.Path).WithArguments("gpg -o /usr/share/keyrings/mongodb-server-" + MongoVersion + ".gpg --dearmor --yes mongodb.asc");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    try
                    {
                        File.Delete(Program.Directory.Cache.Path + "mongodb.asc");
                    }
                    catch { }

                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedAptInstall;
                }

                try
                {
                    File.Delete(Program.Directory.Cache.Path + "mongodb.asc");
                }
                catch { }

                try
                {
                    File.WriteAllText("/etc/apt/sources.list.d/mongodb-org-" + MongoVersion + ".list", "deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-" + MongoVersion + ".gpg ] https://repo.mongodb.org/apt/" + Dist + " " + Codename + "/mongodb-org/" + MongoVersion + " multiverse");

                }
                catch (Exception ex)
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(ex.ToString(), LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedAptInstall;
                }

                cmd = Cli.Wrap("sudo").WithArguments("apt update");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedAptInstall;
                }


                cmd = Cli.Wrap("sudo").WithArguments("apt install -y mongodb-org");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedAptInstall;
                }


                cmd = Cli.Wrap("ps").WithArguments("--no-headers -o comm 1");

                res = await cmd.ExecuteBufferedAsync();
                if (!res.IsSuccess)
                {
                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                    return DatabaseSetupErrorType.FailedSystemProcess;
                }

                Logger.LogMessage("Type: " + res.StandardOutput, LogSeverity.Debug);

                switch (res.StandardOutput.Trim())
                {
                    case "systemd":
                        {
                            cmd = Cli.Wrap("sudo").WithArguments("systemctl start mongod");

                            res = await cmd.ExecuteBufferedAsync();
                            if (!res.IsSuccess)
                            {
                                Logger.LogMessage("--- Database Setup Start ---", LogSeverity.Debug);
                                Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                                Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);

                                if (res.StandardError.Contains("Failed to start"))
                                {
                                    cmd = Cli.Wrap("sudo").WithArguments("systemctl daemon-reload");

                                    res = await cmd.ExecuteBufferedAsync();
                                    if (!res.IsSuccess)
                                    {
                                        Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                                        Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                                        Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                                        return DatabaseSetupErrorType.FailedSystemProcess;
                                    }


                                    cmd = Cli.Wrap("sudo").WithArguments("systemctl start mongod");

                                    res = await cmd.ExecuteBufferedAsync();
                                    if (!res.IsSuccess)
                                    {
                                        Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                                        Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                                        Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                                        return DatabaseSetupErrorType.FailedSystemProcess;
                                    }

                                }
                                else
                                {
                                    Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                                    Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                                    Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
                                    return DatabaseSetupErrorType.FailedSystemProcess;
                                }
                            }

                            cmd = Cli.Wrap("sudo").WithArguments("systemctl enable mongod");

                            res = await cmd.ExecuteBufferedAsync();
                            if (!res.IsSuccess)
                            {
                                Logger.LogMessage("--- Database Setup Error ---", LogSeverity.Debug);
                                Logger.LogMessage(res.StandardError, LogSeverity.Debug);
                                Logger.LogMessage("--- --- --- ---- --- --- ---", LogSeverity.Debug);
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
