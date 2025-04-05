using BattleNET;
using DevSpaceWeb;
using System.Net;

namespace DaRT;

public class RCon
{
    private BattlEyeClient _client;
    private BattlEyeLoginCredentials _credentials;
    private bool _error = false;
    private bool _initialized = false;
    private bool _reconnecting = false;

    private string _pending = "";
    private bool _pendingLeft = false;

    private List<int> _sent = new List<int>();
    private Dictionary<int, string> _received = new Dictionary<int, string>();


    public int SetPlayerTicks = 1000;
    public int SetBanTicks = 10000;
    public string SetPrivateName = "Admin";
    public bool SetShowAdminCalls = true;
    public bool SetUseNameForAdminCalls;



    public bool IsConnected
    {
        get { return _initialized && _client.Connected && !_reconnecting; }
    }
    public bool IsError
    {
        get { return _initialized && _error; }
    }

    public string IsPending
    {
        get { return _pending; }
        set { _pending = value; }
    }
    public bool IsPendingLeft
    {
        get { return _pendingLeft; }
        set { _pendingLeft = value; }
    }

    public bool IsReconnecting
    {
        get { return _reconnecting; }
        set { _reconnecting = value; }
    }

    public RCon()
    {

        // Initializing date
        this.lastsent = new DateTime();
    }

    private DateTime lastsent;

    private int Send(BattlEyeCommand command)
    {
        // Prevent sending packets too fast if necessary
        while ((DateTime.Now - lastsent).TotalMilliseconds <= 10) { Thread.Sleep(10); }

        // Sending command and saving timestamp
        int id = _client.SendCommand(command);
        lastsent = DateTime.Now;

        // Logging sent packet
        if (!_sent.Contains(id))
            _sent.Add(id);
        return id;
    }

    private void Received(int id, string response)
    {
        // Logging received packet
        if (!_received.ContainsKey(id))
        {
            _received.Add(id, response);
        }
    }
    private string GetResponse(int id)
    {
        // Polling for response
        if (_received.ContainsKey(id))
        {
            string response = _received[id];
            this.Remove(id);

            return response;
        }
        else
            return null;
    }
    private bool Remove(int id)
    {
        // Removing packet
        if (_sent.Contains(id))
        {
            _sent.Remove(id);
        }
        else
        {
            if (_received.ContainsKey(id))
                _received.Remove(id);

            return false;
        }

        if (_received.ContainsKey(id))
        {
            _received.Remove(id);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Connect(IPAddress host, int port, string password)
    {
        _credentials = new BattlEyeLoginCredentials
        {
            Host = host,
            Port = port,
            Password = password
        };

        if (_client != null)
        {
            _client.BattlEyeMessageReceived -= HandleMessage;
            _client.BattlEyeConnected -= HandleConnect;
            _client.BattlEyeDisconnected -= HandleDisconnect;
        }

        _client = new BattlEyeClient(_credentials);
        _client.BattlEyeMessageReceived += HandleMessage;
        _client.BattlEyeConnected += HandleConnect;
        _client.BattlEyeDisconnected += HandleDisconnect;
        _client.ReconnectOnPacketLoss = true;

        _initialized = true;
        BEResult = _client.Connect();
    }

    public BattlEyeConnectionResult BEResult;

    public void Disconnect()
    {
        _client.Disconnect();
        _initialized = false;
    }

    public List<string> GetRawBans()
    {
        return null;
        /*
        List<String> bans = new List<String>();

        bool gotBans = false;
        bool bansValid = true;
        bool abort = false;
        int counter = 0;
        int requestCounter = 1;

        do
        {
            gotBans = false;
            bansValid = true;
            abort = false;
            requestCounter = 1;

            bans.Clear();

            counter++;

            while (!gotBans)
            {
                _client.SendCommand("bans");

                Thread.Sleep(500 * requestCounter);

                if (banResult == null)
                    gotBans = false;
                else
                    gotBans = true;

                if (requestCounter >= 5 && !gotBans)
                {
                    if (_form != null) _form.Log("Ban request timed out! (Server didn't respond)");
                    bans.Clear();
                    return bans;
                }
                requestCounter++;
            }

            StringReader reader = new StringReader(banResult);

            String line;
            while ((line = reader.ReadLine()) != null && !abort)
            {
                bans.Add(line);
            }
            bansValid = true;

            reader.Dispose();
            reader.Close();
            gotBans = false;
            banResult = null;
        } while (!bansValid && counter < 3 && !abort);
        if (counter == 3)
        {
            bans.Clear();
            if (_form != null) _form.Log("Ban request timed out! (Data invalid)");
            return bans;
        }
        else
        {
            if (bansValid)
                return bans;
            else
                return bans;
        }
        */
    }
    public List<Player> GetPlayers()
    {
        List<Player> players = new List<Player>();

        int id = this.Send(BattlEyeCommand.Players);

        string response;
        int ticks = 0;
        while ((response = this.GetResponse(id)) == null && ticks < SetPlayerTicks)
        {
            Thread.Sleep(10);
            ticks++;
        }

        if (response == null)
        {
            //if (!_reconnecting)
            //    _form.Log("Player request timed out.", LogType.Console, false);
            return players;
        }

        using (StringReader reader = new StringReader(response))
        {
            string line;
            int row = 0;
            while ((line = reader.ReadLine()) != null)
            {

                row++;
                if (row > 3 && !line.StartsWith("(") && line.Length > 0)
                {
                    String[] items = line.Split([' '], 5, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length == 5)
                    {
                        int number = items[0].StartsWith("#") ? Int32.Parse(items[0].Substring(1)) : Int32.Parse(items[0]);
                        String ip = items[1].Split(':')[0];
                        String ping = items[2];
                        String guid = items[3].Replace("(OK)", "").Replace("(?)", "");
                        String name = items[4];
                        String status = "Unknown";

                        if (guid.Length == 32)
                        {
                            if (guid == "-")
                            {
                                status = "Initializing";
                            }

                            if (name.EndsWith(" (Lobby)"))
                            {
                                name = name.Replace(" (Lobby)", "");
                                status = "Lobby";
                            }
                            else
                                status = "Ingame";

                            players.Add(new Player(number, ip, ping, guid, name, status));
                        }
                        else
                        {
                            // Received malformed player list
                            return new List<Player>();
                        }
                    }
                    else
                    {
                        // Received malformed player list
                        return new List<Player>();
                    }
                }
            }
        }

        return players;
    }

    public List<Ban> GetBans()
    {
        List<Ban> bans = new List<Ban>();

        int id = this.Send(BattlEyeCommand.Bans);

        string response;
        int ticks = 0;
        while ((response = this.GetResponse(id)) == null && ticks < SetBanTicks)
        {
            Thread.Sleep(10);
            ticks++;
        }

        if (response == null)
        {
            //if (!_reconnecting)
            //    _form.Log("Ban request timed out.", LogType.Console, false);
            return bans;
        }

        using (StringReader reader = new StringReader(response))
        {
            String line;
            int row = 0;
            while ((line = reader.ReadLine()) != null)
            {
                row++;
                if (row > 3 && !line.StartsWith("IP Bans:") && !line.StartsWith("[#]") && !line.StartsWith("----------------------------------------------") && line.Length > 0)
                {
                    String[] items = line.Split([' '], 4, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length == 4)
                    {
                        String number = items[0];
                        String ipguid = items[1];
                        String time = items[2];
                        String reason = items[3];

                        if (time == "-")
                            time = "expired";

                        bans.Add(new Ban(number, ipguid, time, reason));
                    }
                    else if (items.Length == 3)
                    {
                        String number = items[0];
                        String ipguid = items[1];
                        String time = items[2];

                        if (time == "-")
                            time = "expired";

                        bans.Add(new Ban(number, ipguid, time, "(No reason)"));
                    }
                }
            }
        }

        return bans;
    }

    public List<string> GetRawPlayers()
    {
        return null;
        /*
        List<String> players = new List<String>();

        bool gotPlayers = false;
        bool playersValid = true;
        int counter = 0;
        int requestCounter = 1;

        do
        {
            gotPlayers = false;
            playersValid = true;
            requestCounter = 1;

            players.Clear();

            counter++;

            while (!gotPlayers)
            {
                _client.SendCommand(BattlEyeCommand.Players);

                Thread.Sleep(500 * requestCounter);

                if (playerResult == null)
                    gotPlayers = false;
                else
                    gotPlayers = true;

                if (requestCounter >= 5 && !gotPlayers)
                {
                    if (_form != null) _form.Log("Player request timed out! (Server didn't respond)");
                    players.Clear();
                    return players;
                }
                requestCounter++;
            }

            StringReader reader = new StringReader(playerResult);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                players.Add(line);
            }
            playersValid = true;

            reader.Dispose();
            reader.Close();
            gotPlayers = false;
            playerResult = null;
        } while (!playersValid);

        if (playersValid)
            return players;
        else
        {
            players.Clear();
            if (_form != null) _form.Log("Player request timed out! (Data invalid)");
            return players;
        }
        */
    }
    public List<RconAdmin>? GetAdmins()
    {
        int packetId = this.Send(BattlEyeCommand.Admins);
        string response;
        int ticks = 0;
        while ((response = this.GetResponse(packetId)) == null && ticks < SetPlayerTicks)
        {
            Thread.Sleep(10);
            ticks++;
        }

        if (response == null)
        {
            return null;
        }

        List<RconAdmin> admins = new List<RconAdmin>();
        using (StringReader reader = new StringReader(response))
        {
            string line;
            int row = 0;
            while ((line = reader.ReadLine()) != null)
            {
                row++;
                if (row > 3)
                {
                    admins.Add(new RconAdmin
                    {
                        Id = int.Parse(line.Split(' ')[0]),
                        Ip = line.Split(' ')[1].Split(':')[0]
                    });
                }
            }
        }

        return admins;
    }

    public void Reassign()
    {
        this.Send(BattlEyeCommand.Reassign);
    }

    public void GetMissions()
    {
        int packetId = this.Send(BattlEyeCommand.Reassign);
        string response;
        int ticks = 0;
        while ((response = this.GetResponse(packetId)) == null && ticks < SetPlayerTicks)
        {
            Thread.Sleep(10);
            ticks++;
        }

        if (response == null)
        {
            return;
        }

        using (StringReader reader = new StringReader(response))
        {
            string line;
            int row = 0;
            while ((line = reader.ReadLine()) != null)
            {
                row++;
                Console.WriteLine($"{row} - {line}");
                if (row > 3)
                {

                }
            }
        }
    }

    public void ReloadConfig()
    {
        _client.SendCommand(BattlEyeCommand.Init);
    }
    public void ReloadScripts()
    {
        _client.SendCommand("loadScripts");
    }
    public void ReloadBans()
    {
        _client.SendCommand("loadBans");
    }
    public void ReloadEvents()
    {
        _client.SendCommand("loadEvents");
    }
    public void LockServer()
    {
        _client.SendCommand("#lock");
    }
    public void UnlockServer()
    {
        _client.SendCommand("#unlock");
    }

    public void RestartServer()
    {
        _client.SendCommand("#restartserver");
    }
    public void ShutdownServer()
    {
        _client.SendCommand("#shutdown");
    }
    public void ExecuteCommand(string command)
    {
        _client.SendCommand(command);
    }

    public void SayPrivate(Message message)
    {
        string name = "";
        if (!string.IsNullOrEmpty(SetPrivateName))
            name = "[" + SetPrivateName + "] ";

        _client.SendCommand(BattlEyeCommand.Say, message.id + " " + name + message.message);
    }
    public void SayGlobal(string message)
    {
        string name = "";
        if (!string.IsNullOrEmpty(SetPrivateName))
            name = "[" + SetPrivateName + "] ";

        _client.SendCommand(BattlEyeCommand.Say, "-1 " + name + message);
    }
    public void KickPlayer(Kick kick)
    {
        string name = "";
        if (!string.IsNullOrEmpty(SetPrivateName))
            name = "[" + SetPrivateName + "] ";

        _client.SendCommand(BattlEyeCommand.Kick, kick.id + " " + name + kick.reason);
    }

    public void BanPlayer(Ban ban)
    {
        string name = "";
        if (!string.IsNullOrEmpty(SetPrivateName))
            name = "[" + SetPrivateName + "] ";

        if (ban.Online)
        {
            if (!string.IsNullOrEmpty(ban.GUID) && !string.IsNullOrEmpty(ban.IP))
            {
                _client.SendCommand(string.Format("banIP {0} {1} {2}", ban.ID, ban.Duration, name + ban.Reason));
                _client.SendCommand(string.Format("addBan {0} {1} {2}", ban.GUID, ban.Duration, name + ban.Reason));
            }
            else if (!string.IsNullOrEmpty(ban.GUID))
                _client.SendCommand(string.Format("ban {0} {1} {2}", ban.ID, ban.Duration, name + ban.Reason));
            else if (!string.IsNullOrEmpty(ban.IP))
                _client.SendCommand(string.Format("banIP {0} {1} {2}", ban.ID, ban.Duration, name + ban.Reason));

        }
        else
        {
            _client.SendCommand(string.Format("addBan {0} {1} {2}", ban.GUID, ban.Duration, name + ban.Reason));
        }
    }
    public void BanIP(BanIP ban)
    {
        _client.SendCommand("banIP " + ban.id + " " + ban.duration + " " + ban.reason);
    }
    public void BanOffline(BanOffline ban)
    {
        _client.SendCommand("addBan " + ban.guid + " " + ban.duration + " " + ban.reason);

    }
    public void UnbanPlayer(string id)
    {
        _client.SendCommand("removeBan " + id);
    }

    public event EventHandler<BattlEyeMessageEventArgs> OnMessage;
    public event EventHandler<Player> PlayerEvent;

    public Stack<BattlEyeMessageEventArgs> CachedMessages = new Stack<BattlEyeMessageEventArgs>(100);
    public List<Player> CachedPlayers = new List<Player>();


    private void HandleMessage(BattlEyeMessageEventArgs args)
    {
        string message = args.Message;

        if (_initialized)
        {
            // Message filtering
            if (args.Id != 256)
                this.Received(args.Id, message);
            else
            {

                args.FilteredMessage = Program.IpRegex.Replace(args.Message, "**.**.**.**");
                // Global chat
                if (message.StartsWith("(Global)"))
                {
                    args.Type = LogType.GlobalChat;

                    //_form.Log(message, , this.IsCall(message));
                }
                // Side chat
                else if (message.StartsWith("(Side)"))
                {
                    args.Type = LogType.SideChat;
                    //_form.Log(message, , this.IsCall(message));
                }
                // Direct chat
                else if (message.StartsWith("(Direct)"))
                {
                    args.Type = LogType.DirectChat;
                    //_form.Log(message, , this.IsCall(message));
                }
                // Vehicle chat
                else if (message.StartsWith("(Vehicle)"))
                {
                    args.Type = LogType.VehicleChat;
                    //    _form.Log(message, , this.IsCall(message));
                }
                // Command chat
                else if (message.StartsWith("(Command)"))
                {
                    args.Type = LogType.CommandChat;

                    //    _form.Log(message, , this.IsCall(message));
                }
                // Group chat
                else if (message.StartsWith("(Group)"))
                {
                    args.Type = LogType.GroupChat;
                    //    _form.Log(message, , this.IsCall(message));
                }
                // Unknown chat
                else if (message.StartsWith("(Unknown)"))
                {
                    args.Type = LogType.UnknownChat;
                    //    _form.Log(message, , this.IsCall(message));
                }
                else if (message.StartsWith("Player #"))
                {

                    if (_pending != "" && message.EndsWith(" " + _pending + " disconnected"))
                        _pendingLeft = true;

                    args.Type = LogType.Console;

                    // Connect/disconnect/kick/ban messages


                    String[] items = message.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (message.EndsWith(" connected"))
                    {
                        Logger.LogMessage("Add Player", LogSeverity.Debug);

                        int Number = int.Parse(items[1].Substring(1));
                        string Name = items[2];
                        string Ip = items[3].Replace("(", "").Replace(")", "").Split(':')[0];
                        CachedPlayers.Add(new Player(Number,
                            Ip,
                            "-",
                            "",
                            Name,
                            "Connecting"));
                        //PlayerEvent?.Invoke(this, null);
                    }
                    else if (message.EndsWith(" disconnected"))
                    {
                        Logger.LogMessage("Remove Player", LogSeverity.Debug);
                        try
                        {
                            CachedPlayers.Remove(CachedPlayers.First(x => x.number == int.Parse(items[1].Substring(1))));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        //PlayerEvent?.Invoke(this, null);
                    }

                }
                else if (message.StartsWith("Verified GUID ("))
                {
                    args.Type = LogType.Console;

                    // GUID verification messages
                    String[] items = message.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    int Number = int.Parse(items[5].Substring(1));
                    Player? FindPlayer = CachedPlayers.FirstOrDefault(x => x.number == Number);
                    if (FindPlayer != null)
                        FindPlayer.guid = items[2].Replace("(", "").Replace(")", "");
                }
                else if (message.StartsWith("RCon admin #"))
                {
                    // Admin login
                    if (message.EndsWith("logged in"))
                        args.Type = LogType.Console;
                    else
                        args.Type = LogType.AdminChat;

                }
                else if (message.StartsWith("Failed to open") || message.StartsWith("Incompatible filter file"))
                {
                    args.Type = LogType.Console;
                    // Log errors
                }
                // Scripts log
                else if (message.StartsWith("Scripts Log:"))
                {
                    args.Type = LogType.ScriptsLog;
                }
                // CreateVehicle log
                else if (message.StartsWith("CreateVehicle Log:"))
                {
                    args.Type = LogType.CreateVehicleLog;
                }
                // DeleteVehicle log
                else if (message.StartsWith("DeleteVehicle Log:"))
                {
                    args.Type = LogType.DeleteVehicleLog;
                }
                // PublicVariable log
                else if (message.StartsWith("PublicVariable Log:"))
                {
                    args.Type = LogType.PublicVariableLog;
                }
                // PublicVariableVal log
                else if (message.StartsWith("PublicVariable Value Log:"))
                {
                    args.Type = LogType.PublicVariableValLog;
                }
                // RemoteExec log
                else if (message.StartsWith("RemoteExec Log:"))
                {
                    args.Type = LogType.RemoteExecLog;
                }
                // RemoteControl log
                else if (message.StartsWith("RemoteControl Log:"))
                {
                    args.Type = LogType.RemoteControlLog;
                }
                // SetDamage log
                else if (message.StartsWith("SetDamage Log:"))
                {
                    args.Type = LogType.SetDamageLog;
                }
                // SetVariable log
                else if (message.StartsWith("SetVariable Log:"))
                {
                    args.Type = LogType.SetVariableLog;
                }
                // SetVariableVal log
                else if (message.StartsWith("SetVariable Value Log:"))
                {
                    args.Type = LogType.SetVariableValLog;
                }
                // SetPos log
                else if (message.StartsWith("SetPos Log:"))
                {
                    args.Type = LogType.SetPosLog;
                }
                // AddMagazineCargo log
                else if (message.StartsWith("AddMagazineCargo Log:"))
                {
                    args.Type = LogType.AddMagazineCargoLog;
                }
                // AddWeaponCargo log
                else if (message.StartsWith("AddWeaponCargo Log:"))
                {
                    args.Type = LogType.AddWeaponCargoLog;
                }
                // AddBackpackCargo log
                else if (message.StartsWith("AddBackpackCargo Log:"))
                {
                    args.Type = LogType.AddBackpackCargoLog;
                }
                // AttachTo log
                else if (message.StartsWith("AttachTo Log:"))
                {
                    args.Type = LogType.AttachToLog;
                }
                // MPEventHandler log
                else if (message.StartsWith("MPEventHandler Log:"))
                {
                    args.Type = LogType.MPEventHandlerLog;
                }
                // TeamSwitch log
                else if (message.StartsWith("TeamSwitch Log:"))
                {
                    args.Type = LogType.TeamSwitchLog;
                }
                // SelectPlayer log
                else if (message.StartsWith("SelectPlayer Log:"))
                {
                    args.Type = LogType.SelectPlayerLog;
                }
                // WaypointCondition log
                else if (message.StartsWith("WaypointCondition Log:"))
                {
                    args.Type = LogType.WaypointConditionLog;
                }
                // WaypointStatement log
                else if (message.StartsWith("WaypointStatement Log:"))
                {
                    args.Type = LogType.WaypointStatementLog;
                }
                else
                {
                    //if (_form != null) _form.Log("UNKNOWN: " + message, LogType.Debug, false);
                    return;
                }

                if (CachedMessages.Count >= 100)
                    CachedMessages.Pop();
                CachedMessages.Push(args);
                OnMessage?.Invoke(this, args);

            }
        }
    }

    private void HandleConnect(BattlEyeConnectEventArgs args)
    {
        switch (args.ConnectionResult)
        {
            case BattlEyeConnectionResult.Success:
                _ = Task.Run(() =>
                {
                    CachedPlayers = GetPlayers();
                });
                //if (Settings.Default.showConnectMessages && _form != null)
                //{
                //    if (!_reconnecting && _form.connect.Enabled == true)
                //        _form.Log("Connected!", LogType.Console, false);
                //    else
                //        _form.Log("Reconnected!", LogType.Console, false);
                //}
                _error = false;
                break;
            case BattlEyeConnectionResult.InvalidLogin:
                //if (_form != null) _form.Log("Login invalid!", LogType.Console, false);
                _error = true;
                break;
            case BattlEyeConnectionResult.ConnectionFailed:
                //if (_form.connect.Enabled)
                //    if (_form != null) _form.Log("Failed to connect. Please make sure that you properly set a password in beserver.cfg and the server is running.", LogType.Console, false);

                _error = true;
                break;
            default:
                //if (_form != null) _form.Log("Unknown error.", LogType.Console, false);

                _error = true;
                break;
        }
    }


    private void HandleDisconnect(BattlEyeDisconnectEventArgs args)
    {
        switch (args.DisconnectionType)
        {
            case BattlEyeDisconnectionType.ConnectionLost:
                CachedPlayers.Clear();
                if (!_reconnecting)
                {
                    //if (_form != null) _form.Log("Connection to server was lost. Attempting to reconnect...", LogType.Console, false);
                    this.Reconnect();
                }
                break;
            case BattlEyeDisconnectionType.Manual:
                CachedPlayers.Clear();
                // Handle manual reconnect
                break;

            case BattlEyeDisconnectionType.SocketException:
                CachedPlayers.Clear();

                if (!_reconnecting)
                {
                    //if (_form != null) _form.Log("It appears that the server went down. Attempting to reconnect...", LogType.Console, false);
                    this.Reconnect();
                }
                break;
            default:
                CachedPlayers.Clear();
                //if (_form != null) _form.Log("Unknown error.", LogType.Console, false);
                break;
        }
    }

    private bool IsCall(string message)
    {
        try
        {
            if (SetShowAdminCalls)
            {
                message = message.Split([':'], 2, StringSplitOptions.RemoveEmptyEntries)[1];

                bool important = message.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0;

                if (SetUseNameForAdminCalls && !string.IsNullOrEmpty(SetPrivateName) && !important)
                    important = important || message.IndexOf(SetPrivateName, StringComparison.OrdinalIgnoreCase) >= 0;

                return important;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }


    Thread reconnectThread = null;

    private void Reconnect()
    {
        _reconnecting = true;
        reconnectThread = new Thread(new ThreadStart(HandleReconnect));
        reconnectThread.IsBackground = true;
        reconnectThread.Start();
    }
    private void HandleReconnect()
    {
        while (_reconnecting && _initialized && !_client.Connected)
        {
            Thread.Sleep(5000);
            this.Disconnect();
            this.Connect(_credentials.Host, _credentials.Port, _credentials.Password);
        }
        _reconnecting = false;
    }

    public bool isIP(String ip)
    {
        try
        {
            IPAddress.Parse(ip);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
