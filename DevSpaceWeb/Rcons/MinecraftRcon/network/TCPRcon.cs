using System.Net.Sockets;
using System.Text;

//!Classes directly related to the minecraft server.
namespace LibMCRcon.RCon;

/// <summary>
/// Extending a Queue of RConPacket(s), a TCP stream connection with background threads for asyncronous send/receive.
/// </summary>
public class TCPRcon : Queue<RconPacket>
{


    public enum TCPState { IDLE, CONNECTING, CONNECTED, CLOSING, CLOSED, ABORTED };
    public enum RConState { IDLE, AUTHENTICATE, READY, NETWORK_FAIL, AUTHENTICATE_FAIL };
    public string LastTCPError { get; set; }

    public string RConHost { get; set; }
    public string RConPass { get; set; }
    public int RConPort { get; set; }

    public TCPState StateTCP { get; private set; }
    public RConState StateRCon { get; private set; }

    protected bool AbortTCP { get; set; }

    Thread? bgCommThread;
    Queue<RconPacket> cmdQue = new Queue<RconPacket>();

    int sessionID = -1;
    /// <summary>
    /// Default constructor, will still need RCon server url, password and port.
    /// </summary>
    public TCPRcon()
        : base()
    {



    }
    /// <summary>
    /// Create a TCPRcon connection.  Does not open on creation.
    /// </summary>
    /// <param name="MineCraftServer">DNS address of the rcon server.</param>
    /// <param name="port">Port RCon is listening to on the server.</param>
    /// <param name="password">Configured password for the RCon server.</param>
    public TCPRcon(string host, int port, string password)
        : base()
    {
        RConHost = host;
        RConPort = port;
        RConPass = password;

    }

    /// <summary>
    /// Asynchronous que of command.  Will be sent and response collected as soon as possible.
    /// </summary>
    /// <param name="Command">The string command to send, no larger than rcon message specification for minecraft's implementation.</param>
    public void QueCommand(String Command)
    {
        cmdQue.Enqueue(RconPacket.CmdPacket(Command, sessionID));
    }

    /// <summary>
    /// Start the asynchronous communication process.
    /// </summary>
    /// <returns>True of successfully started, otherwise false.</returns>
    public bool StartComms()
    {

        if (bgCommThread != null)
            if (bgCommThread.IsAlive)
            {
                StopComms();
            }

        bgCommThread = null;
        TimeCheck tc;

        tc = new TimeCheck(10000);


        bgCommThread = new Thread(ConnectAndProcess)
        {
            IsBackground = true
        };

        StateTCP = TCPState.IDLE;
        StateRCon = RConState.IDLE;

        bgCommThread.Start();
        while (tc.Expired == false)
        {

            if (StateTCP == TCPState.CONNECTED)
                if (StateRCon == RConState.READY)
                    return true;

            if (StateTCP == TCPState.ABORTED)
                break;

        }

        return false;
    }
    /// <summary>
    /// Stop communication and close all connections.  Will block until complete or timed out.
    /// </summary>
    public void StopComms()
    {
        StateTCP = TCPState.CLOSING;
        AbortTCP = true;
        if (bgCommThread != null)
            if (bgCommThread.IsAlive)
                bgCommThread.Join();

        bgCommThread = null;
        StateRCon = RConState.IDLE;
    }
    /// <summary>
    /// True if connected and active.
    /// </summary>
    public bool IsConnected { get { return StateTCP == TCPState.CONNECTED; } }
    /// <summary>
    /// True if the asynchronous thread is running.
    /// </summary>
    public bool IsStarted { get { return bgCommThread.IsAlive; } }
    /// <summary>
    /// True if the connection is open and the queue is ready for commands.
    /// </summary>
    public bool IsReadyForCommands { get { return StateTCP == TCPState.CONNECTED && StateRCon == RConState.READY; } }

    private void ConnectAndProcess()
    {

        DateTime transmitLatch = DateTime.Now.AddMilliseconds(-1);
        Random r = new Random();


        using (TcpClient cli = new TcpClient())
        {


            sessionID = r.Next(1, int.MaxValue) + 1;

            StateTCP = TCPState.CONNECTING;
            StateRCon = RConState.IDLE;

            AbortTCP = false;
            try
            {

                cli.ConnectAsync(RConHost, RConPort).Wait(5000);

                if (cli.Connected == false)
                {

                    AbortTCP = true;
                    StateTCP = TCPState.ABORTED;
                    StateRCon = RConState.NETWORK_FAIL;
                    return;
                }

                StateTCP = TCPState.CONNECTED;
                StateRCon = RConState.AUTHENTICATE;

                RconPacket auth = RconPacket.AuthPacket(RConPass, sessionID);
                auth.SendToNetworkStream(cli.GetStream());

                if (auth.IsBadPacket == false)
                {
                    RconPacket resp = new RconPacket();
                    resp.ReadFromNetworkSteam(cli.GetStream());

                    if (resp.IsBadPacket == false)
                    {
                        if (resp.SessionID == -1 && resp.ServerType == 2)
                            StateRCon = RConState.AUTHENTICATE_FAIL;

                    }
                    else
                        StateRCon = RConState.NETWORK_FAIL;
                }


                if (StateTCP == TCPState.CONNECTED)
                {
                    if (cli.Connected == false)
                    {
                        StateTCP = TCPState.ABORTED;
                        AbortTCP = true;

                    }

                    if (StateRCon != RConState.AUTHENTICATE)
                    {
                        AbortTCP = true;
                        StateTCP = TCPState.ABORTED;
                        StateRCon = RConState.AUTHENTICATE_FAIL;
                        return;
                    }
                    else
                        StateRCon = RConState.READY;
                }


                Comms(cli);

                AbortTCP = true;
                StateTCP = TCPState.ABORTED;
            }

            catch (Exception e)
            {
                LastTCPError = e.Message;
                AbortTCP = true;
                StateRCon = RConState.NETWORK_FAIL;
            }

            finally
            {
                if (cli.Connected == true)
                    cli.Close();
            }


        }

    }

    private void Comms(TcpClient cli)
    {

        TimeCheck tc = new TimeCheck();
        Int32 dT = 200;

        cli.SendTimeout = 5000;
        cli.ReceiveTimeout = 20000;

        try
        {

            if (cli.Connected == false) //Not connected, shut it down...
            {
                StateRCon = RConState.NETWORK_FAIL;
                StateTCP = TCPState.ABORTED;
                AbortTCP = true;
            }


            tc.Reset(dT);

            while (AbortTCP == false)
            {

                do
                {

                    if (cli.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (cli.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            //client seems to be closed - lets close it

                            StateTCP = TCPState.CLOSED;
                            StateRCon = RConState.NETWORK_FAIL;
                            AbortTCP = true;
                            break;
                        }

                    }



                    if (cli.Available > 0)
                    {


                        RconPacket resp = new RconPacket();
                        resp.ReadFromNetworkSteam(cli.GetStream());

                        if (resp.IsBadPacket == true)
                        {
                            StateTCP = TCPState.ABORTED;
                            StateRCon = RConState.NETWORK_FAIL;
                            AbortTCP = true;
                            break;

                        }

                        if (Count > 1500)
                        {
                            StateRCon = RConState.IDLE;
                            StateTCP = TCPState.ABORTED;
                            AbortTCP = true;
                            break;
                        }
                        else
                        {

                            Enqueue(resp);
                            StateRCon = RConState.READY;
                        }

                        if (tc.Expired == false)
                            tc.Reset(dT);
                    }

                    Thread.Sleep(1);


                } while (tc.Expired == false || cli.Available > 0);

                if (AbortTCP == true)
                    break;


                if (cmdQue.Count > 0)
                {
                    RconPacket Cmd = cmdQue.Dequeue();

                    Cmd.SendToNetworkStream(cli.GetStream());
                    tc.Reset(dT);
                }

                Thread.Sleep(1);
            }
        }

        catch (Exception ee)
        {
            AbortTCP = true;
            StringBuilder sb = new StringBuilder();

            Exception? ex = ee;
            do
            {
                sb.AppendLine(ex.Message);
                ex = ee.InnerException;
            } while (ex != null);



            LastTCPError = sb.ToString();

            StateTCP = TCPState.ABORTED;
            StateRCon = RConState.NETWORK_FAIL;
        }

        if (cli.Connected)
            cli.Close();

    }

    private void ShutDownComms()
    {

        AbortTCP = true;
        if (bgCommThread.IsAlive)
            bgCommThread.Join();
        else
        {
            StateTCP = TCPState.ABORTED;
            StateRCon = RConState.IDLE;
        }

    }
    /// <summary>
    /// Execute a command and wait for a response, blocking main calling thread.  Once response given return.
    /// </summary>
    /// <param name="formatedCmd">Allows for C# style formated string, final result in a minecraft style command.</param>
    /// <param name="args">Same arguments supplied to the string.format function.</param>
    /// <returns>If command is sent and a response given, the repsonse is removed from the response que an returned.</returns>
    public string ExecuteCmd(string formatedCmd, params object[] args)
    {
        return ExecuteCmd(string.Format(formatedCmd, args));
    }
    /// <summary>
    /// Execute a command and wait for a response, blocking main calling thread.  Once response given return.
    /// </summary>
    /// <param name="Cmd">Command to be sent to the rcon server for the minecraft server to execute.</param>
    /// <returns>If command is sent and a response given, the repsonse is removed from the response que an returned.</returns>
    public string ExecuteCmd(string Cmd)
    {

        if (AbortTCP == true)
            return "RCON_ABORTED";

        RconPacket p;
        StringBuilder sb = new StringBuilder();

        TimeCheck tc = new TimeCheck();

        QueCommand(Cmd);

        while (Count == 0)
        {
            Thread.Sleep(100);
            if (AbortTCP == true) break;
            if (tc.Expired == true) break;
        }

        while (Count > 0)
        {
            p = Dequeue();
            sb.Append(p.Response);

            if (AbortTCP == true) break;
        }

        return sb.ToString();

    }



}

public class TCPRconAsync : Queue<RconPacket>
{

    public enum TCPState { IDLE, CONNECTING, CONNECTED, CLOSING, CLOSED, ABORTED };
    public enum RConState { IDLE, AUTHENTICATE, READY, NETWORK_FAIL, AUTHENTICATE_FAIL };
    public string LastTCPError { get; set; }

    public string RConHost { get; set; }
    public string RConPass { get; set; }
    public int RConPort { get; set; }

    public TCPState StateTCP { get; private set; }
    public RConState StateRCon { get; private set; }

    protected bool AbortTCP { get; set; }

    Task? bgTask;

    Queue<RconPacket> cmdQue = new Queue<RconPacket>();

    int sessionID = -1;
    /// <summary>
    /// Default constructor, will still need RCon server url, password and port.
    /// </summary>
    public TCPRconAsync()
        : base()
    {



    }
    /// <summary>
    /// Create a TCPRcon connection.  Does not open on creation.
    /// </summary>
    /// <param name="MineCraftServer">DNS address of the rcon server.</param>
    /// <param name="port">Port RCon is listening to on the server.</param>
    /// <param name="password">Configured password for the RCon server.</param>
    public TCPRconAsync(string host, int port, string password)
        : base()
    {
        RConHost = host;
        RConPort = port;
        RConPass = password;

    }

    /// <summary>
    /// Asynchronous que of command.  Will be sent and response collected as soon as possible.
    /// </summary>
    /// <param name="Command">The string command to send, no larger than rcon message specification for minecraft's implementation.</param>
    public void QueCommand(String Command)
    {
        cmdQue.Enqueue(RconPacket.CmdPacket(Command, sessionID));
    }

    /// <summary>
    /// Clones the current connection allowing another session to the rcon server.
    /// </summary>
    /// <returns>Return TCPRcon with the same host,port, and password.</returns>
    public async Task<TCPRconAsync?> CopyConnection()
    {

        TCPRconAsync r = new TCPRconAsync(RConHost, RConPort, RConPass);
        if (await r.StartComms() == false)
            return null;

        return r;
    }

    /// <summary>
    /// Start the asynchronous communication process.
    /// </summary>
    /// <returns>True of successfully started, otherwise false.</returns>
    public async Task<bool> StartComms()
    {

        if (bgTask != null)
            await StopComms();

        StateTCP = TCPState.IDLE;
        StateRCon = RConState.IDLE;

        TcpClient? cli = await Connect();
        if (cli == null)
            return false;

        if (StateTCP == TCPState.CONNECTED)
            if (StateRCon == RConState.READY)
            {
                bgTask = Process(cli);
                return true;
            }

        return false;
    }
    /// <summary>
    /// Stop communication and close all connections.  Will block until complete or timed out.
    /// </summary>
    public async Task StopComms()
    {

        StateTCP = TCPState.CLOSING;
        AbortTCP = true;

        if (bgTask != null)
        {
            await bgTask;
            bgTask = null;
        }

        StateRCon = RConState.IDLE;
    }
    /// <summary>
    /// True if connected and active.
    /// </summary>
    public bool IsConnected { get { return StateTCP == TCPState.CONNECTED; } }
    /// <summary>
    /// True if the asynchronous thread is running.
    /// </summary>
    public bool IsStarted { get { return bgTask != null; } }
    /// <summary>
    /// True if the connection is open and the queue is ready for commands.
    /// </summary>
    public bool IsReadyForCommands { get { return StateTCP == TCPState.CONNECTED && StateRCon == RConState.READY; } }

    private async Task<TcpClient?> Connect()
    {
        Random r = new Random();
        TcpClient cli = new TcpClient();

        sessionID = r.Next(1, int.MaxValue) + 1;

        StateTCP = TCPState.CONNECTING;
        StateRCon = RConState.IDLE;

        AbortTCP = false;
        try
        {

            await Task.WhenAny(cli.ConnectAsync(RConHost, RConPort), Task.Delay(5000));

            if (cli.Connected == false)
            {
                cli.Close();


                AbortTCP = true;
                StateTCP = TCPState.ABORTED;
                StateRCon = RConState.NETWORK_FAIL;
                return null;
            }

            StateTCP = TCPState.CONNECTED;
            StateRCon = RConState.AUTHENTICATE;

            RconPacket auth = RconPacket.AuthPacket(RConPass, sessionID);
            await auth.SendToNetworkStreamAsync(cli.GetStream());

            if (auth.IsBadPacket == false)
            {
                RconPacket resp = new RconPacket();
                await resp.ReadFromNetworkSteamAsync(cli.GetStream());

                if (resp.IsBadPacket == false)
                {
                    if (resp.SessionID == -1 && resp.ServerType == 2)
                        StateRCon = RConState.AUTHENTICATE_FAIL;

                }
                else
                    StateRCon = RConState.NETWORK_FAIL;
            }


            if (StateTCP == TCPState.CONNECTED)
            {
                if (cli.Connected == false)
                {
                    cli.Close();

                    StateTCP = TCPState.ABORTED;
                    AbortTCP = true;

                    return null;
                }

                if (StateRCon != RConState.AUTHENTICATE)
                {
                    AbortTCP = true;
                    StateTCP = TCPState.ABORTED;
                    StateRCon = RConState.AUTHENTICATE_FAIL;
                    cli.Close();
                    return null;
                }
                else
                    StateRCon = RConState.READY;
            }

            return cli;
        }
        catch (Exception e)
        {
            LastTCPError = e.Message;
            AbortTCP = true;
            StateRCon = RConState.NETWORK_FAIL;

            if (cli.Connected == true)
                cli.Close();
        }

        return null;
    }

    private async Task Process(TcpClient cli)
    {


        using (cli)
        {

            try
            {

                await CommsAsync(cli);

                AbortTCP = true;
                StateTCP = TCPState.ABORTED;
            }
            catch (Exception e)
            {
                LastTCPError = e.Message;
                AbortTCP = true;
                StateRCon = RConState.NETWORK_FAIL;
            }

            finally
            {
                if (cli.Connected == true)
                    cli.Close();
            }


        }

    }
    private async Task CommsAsync(TcpClient cli)
    {

        TimeSpan dT = TimeSpan.FromSeconds(5);


        cli.SendTimeout = 5000;
        cli.ReceiveTimeout = 20000;

        async Task FetchNextCommand()
        {
            if (cmdQue.Count > 0)
            {
                RconPacket Cmd = cmdQue.Dequeue();
                await Cmd.SendToNetworkStreamAsync(cli.GetStream());

            }
        }
        async Task<RconPacket> FetchNextPacket()
        {
            RconPacket resp = new RconPacket();
            await resp.ReadFromNetworkSteamAsync(cli.GetStream());
            return resp;
        }

        try
        {

            if (cli.Connected == false) //Not connected, shut it down...
            {
                StateRCon = RConState.NETWORK_FAIL;
                StateTCP = TCPState.ABORTED;
                AbortTCP = true;
            }



            while (AbortTCP == false)
            {

                if (cli.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (cli.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        //client seems to be closed - lets close it

                        StateTCP = TCPState.CLOSED;
                        StateRCon = RConState.NETWORK_FAIL;
                        AbortTCP = true;
                        break;
                    }

                }

                Task NextSend = FetchNextCommand();

                if (cli.Available > 0)
                {
                    RconPacket resp = await FetchNextPacket();

                    if (resp.IsBadPacket)
                    {
                        StateTCP = TCPState.ABORTED;
                        StateRCon = RConState.NETWORK_FAIL;
                        AbortTCP = true;
                        break;
                    }
                    else
                        Enqueue(resp);

                    if (Count > 1500)
                    {

                        //There are over 1500 responses in the queue - too much...
                        StateRCon = RConState.IDLE;
                        StateTCP = TCPState.ABORTED;
                        AbortTCP = true;
                        break;
                    }
                    else
                    {
                        StateRCon = RConState.READY;
                    }

                }

                await NextSend;
                await Task.Delay(1);


                if (AbortTCP == true)
                    break;
            }
        }

        catch (Exception ee)
        {
            AbortTCP = true;
            StringBuilder sb = new StringBuilder();

            Exception? ex = ee;
            do
            {
                sb.AppendLine(ex.Message);
                ex = ee.InnerException;
            } while (ex != null);



            LastTCPError = sb.ToString();

            StateTCP = TCPState.ABORTED;
            StateRCon = RConState.NETWORK_FAIL;
        }

        if (cli.Connected)
            cli.Close();

    }

    /// <summary>
    /// Execute a command and wait for a response, blocking main calling thread.  Once response given return.
    /// </summary>
    /// <param name="formatedCmd">Allows for C# style formated string, final result in a minecraft style command.</param>
    /// <param name="args">Same arguments supplied to the string.format function.</param>
    /// <returns>If command is sent and a response given, the repsonse is removed from the response que an returned.</returns>
    public async Task<string> ExecuteCmd(string formatedCmd, params object[] args)
    {
        return await ExecuteCmd(string.Format(formatedCmd, args));
    }
    /// <summary>
    /// Execute a command and wait for a response, blocking main calling thread.  Once response given return.
    /// </summary>
    /// <param name="Cmd">Command to be sent to the rcon server for the minecraft server to execute.</param>
    /// <returns>If command is sent and a response given, the repsonse is removed from the response que an returned.</returns>

    public Task<string> SayPlayer(string player, string message) => ExecuteCmd($"tell {player} {message}");

    public Task<string> SayGlobal(string message) => ExecuteCmd("say " + message);

    public async Task<string> ExecuteCmd(string Cmd)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        TimeSpan ts = TimeSpan.FromSeconds(5);

        sw.Restart();

        if (AbortTCP == true)
            return "RCON_ABORTED";

        RconPacket p;
        StringBuilder sb = new StringBuilder();

        TimeCheck tc = new TimeCheck();

        QueCommand(Cmd);

        while (Count == 0)
        {
            await Task.Delay(100);

            if (AbortTCP == true) break;
            if (sw.Elapsed > ts)
                break;
        }

        while (Count > 0)
        {
            p = Dequeue();
            sb.Append(p.Response);

            if (AbortTCP == true) break;
        }

        return sb.ToString();

    }



}
