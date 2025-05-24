/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleNET;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class BattlEyeClient
{
    private Socket _socket;
    private DateTime _packetSent;
    private DateTime _packetReceived;
    private BattlEyeDisconnectionType? _disconnectionType;
    private bool _keepRunning;
    private int _sequenceNumber;
    private int _currentPacket;
    private SortedDictionary<int, string?[]> _packetQueue;
    private BattlEyeLoginCredentials _loginCredentials;

    public bool Connected => _socket != null && _socket.Connected;

    public bool ReconnectOnPacketLoss
    {
        get;
        set;
    }

    public int CommandQueue => _packetQueue.Count;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BattlEyeClient(BattlEyeLoginCredentials loginCredentials)
    {
        _loginCredentials = loginCredentials;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public BattlEyeConnectionResult Connect()
    {
        return ConnectInternal(100);
    }

    private BattlEyeConnectionResult ConnectInternal(int counter)
    {
        _packetSent = DateTime.Now;
        _packetReceived = DateTime.Now;

        _sequenceNumber = 0;
        _currentPacket = -1;
        _packetQueue = [];
        _keepRunning = true;

        IPEndPoint remoteEp = new IPEndPoint(_loginCredentials.Host, _loginCredentials.Port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            ReceiveBufferSize = int.MaxValue,
            ReceiveTimeout = 15000
        };

        try
        {
            _socket.Connect(remoteEp);

            if (SendLoginPacket(_loginCredentials.Password) == BattlEyeCommandResult.Error)
                return BattlEyeConnectionResult.ConnectionFailed;

            byte[] bytesReceived = new Byte[4096];

            _socket.Receive(bytesReceived, bytesReceived.Length, 0);

            if (bytesReceived[7] == 0x00)
            {
                if (bytesReceived[8] == 0x01)
                {
                    OnConnect(_loginCredentials, BattlEyeConnectionResult.Success);

                    Receive();
                }
                else
                {
                    OnConnect(_loginCredentials, BattlEyeConnectionResult.InvalidLogin);
                    return BattlEyeConnectionResult.InvalidLogin;
                }
            }
        }
        catch
        {
            if (_disconnectionType == BattlEyeDisconnectionType.ConnectionLost)
            {
                Disconnect(BattlEyeDisconnectionType.ConnectionLost);

                if (counter > 0)
                {
                    return ConnectInternal(counter - 1);
                }

                return BattlEyeConnectionResult.ConnectionFailed;
            }
            else
            {
                OnConnect(_loginCredentials, BattlEyeConnectionResult.ConnectionFailed);
                return BattlEyeConnectionResult.ConnectionFailed;
            }
        }

        return BattlEyeConnectionResult.Success;
    }

    private BattlEyeCommandResult SendLoginPacket(string command)
    {
        try
        {
            if (!_socket.Connected)
                return BattlEyeCommandResult.NotConnected;

            byte[] packet = ConstructPacket(BattlEyePacketType.Login, 0, command);


            _socket.Send(packet);

            _packetSent = DateTime.Now;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BattlEyeCommandResult.Error;
        }


        return BattlEyeCommandResult.Success;
    }

    private BattlEyeCommandResult SendAcknowledgePacket(string command)
    {
        try
        {
            if (!_socket.Connected) return BattlEyeCommandResult.NotConnected;

            byte[] packet = ConstructPacket(BattlEyePacketType.Acknowledge, 0, command);
            _socket.Send(packet);

            _packetSent = DateTime.Now;
        }
        catch
        {
            return BattlEyeCommandResult.Error;
        }

        return BattlEyeCommandResult.Success;
    }

    public int SendCommand(string command, bool log = true)
    {
        return SendCommandPacket(command, log);
    }

    private int SendCommandPacket(string? command, bool log = true)
    {
        int packetID = _sequenceNumber;
        _sequenceNumber = (_sequenceNumber == 255) ? 0 : _sequenceNumber + 1;

        try
        {
            if (!_socket.Connected)
                return 256;

            byte[] packet = ConstructPacket(BattlEyePacketType.Command, packetID, command);

            _packetSent = DateTime.Now;

            if (log)
            {
                _packetQueue.Add(packetID, [command, _packetSent.ToString(CultureInfo.InvariantCulture)]);
            }
            else
            {
                SendPacket(packet);
            }
        }
        catch
        {
            return 256;
        }

        return packetID;
    }

    public int SendCommand(BattlEyeCommand command, string parameters = "")
    {
        return SendCommandPacket(command, parameters);
    }

    private int SendCommandPacket(BattlEyeCommand command, string parameters = "")
    {
        int packetID = _sequenceNumber;
        _sequenceNumber = (_sequenceNumber == 255) ? 0 : _sequenceNumber + 1;

        try
        {
            if (!_socket.Connected)
                return 256;

            ConstructPacket(BattlEyePacketType.Command, packetID, Helpers.StringValueOf(command) + parameters);

            _packetSent = DateTime.Now;

            _packetQueue.Add(packetID, [Helpers.StringValueOf(command) + parameters, _packetSent.ToString(CultureInfo.InvariantCulture)]);
        }
        catch
        {
            return 256;
        }

        return packetID;
    }

    private void SendPacket(byte[] packet)
    {
        _socket.Send(packet);
    }

    private byte[] ConstructPacket(BattlEyePacketType packetType, int sequenceNumber, string? command)
    {
        string type;


        switch (packetType)
        {
            case BattlEyePacketType.Login:
                type = Helpers.Hex2Ascii("FF00");
                break;
            case BattlEyePacketType.Command:
                type = Helpers.Hex2Ascii("FF01");
                break;
            case BattlEyePacketType.Acknowledge:
                type = Helpers.Hex2Ascii("FF02");
                break;
            default:
                return [];
        }


        if (packetType != BattlEyePacketType.Acknowledge)
        {
            if (command != null)
                command = Encoding.GetEncoding(1252).GetString(Encoding.UTF8.GetBytes(command));

        }

        string count = Helpers.Bytes2String([(byte)sequenceNumber]);

        byte[] byteArray = new CRC32().ComputeHash(Helpers.String2Bytes(type + ((packetType != BattlEyePacketType.Command) ? "" : count) + command));

        string hash = new string(Helpers.Hex2Ascii(BitConverter.ToString(byteArray).Replace("-", "")).ToCharArray().Reverse().ToArray());

        string packet = "BE" + hash + type + ((packetType != BattlEyePacketType.Command) ? "" : count) + command;


        return Helpers.String2Bytes(packet);
    }

    public void Disconnect()
    {
        _keepRunning = false;

        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        OnDisconnect(_loginCredentials, BattlEyeDisconnectionType.Manual);
    }

    private void Disconnect(BattlEyeDisconnectionType? disconnectionType)
    {
        if (disconnectionType == BattlEyeDisconnectionType.ConnectionLost)
            _disconnectionType = BattlEyeDisconnectionType.ConnectionLost;

        _keepRunning = false;

        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        if (disconnectionType != null)
            OnDisconnect(_loginCredentials, disconnectionType);
    }

    private async void Receive()
    {
        StateObject state = new StateObject { WorkSocket = _socket };

        _disconnectionType = null;

        _socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);

        while (_socket.Connected && _keepRunning)
        {
            int timeoutClient = (int)(DateTime.Now - _packetSent).TotalSeconds;
            int timeoutServer = (int)(DateTime.Now - _packetReceived).TotalSeconds;

            if (timeoutClient >= 5)
            {
                if (timeoutServer >= 20)
                {
                    Disconnect(BattlEyeDisconnectionType.ConnectionLost);
                    _keepRunning = true;
                }
                else
                {
                    if (_packetQueue.Count == 0)
                    {
                        SendCommandPacket(null, false);
                    }
                }
            }

            if (_socket.Connected && _packetQueue.Count > 0 && _socket.Available == 0)
            {
                try
                {
                    int key = _packetQueue.First().Key;

                    if (_currentPacket == -1 || !_packetQueue.ContainsKey(_currentPacket))
                    {
                        _currentPacket = key;
                        string value = _packetQueue[key][0];
                        SendPacket(ConstructPacket(BattlEyePacketType.Command, key, value));
                    }
                }
                catch
                {
                    // Prevent possible crash when packet is received at the same moment it's trying to resend it.
                }
            }

            await Task.Delay(250);
        }

        if (!_socket.Connected)
        {
            if (ReconnectOnPacketLoss && _keepRunning)
            {
                Connect();
            }
            else if (!_keepRunning)
            {
                //let the thread finish without further action
            }
            else
            {
                OnDisconnect(_loginCredentials, BattlEyeDisconnectionType.ConnectionLost);
            }
        }

    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {

            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state?.WorkSocket;

            // this method can be called from the middle of a .Disconnect() call
            // test with Debug > Exception > CLR exs on
            if (client == null || state == null || !client.Connected)
            {
                return;
            }

            int bytesRead = client.EndReceive(ar);

            if (state.Buffer[7] == 0x02)
            {
                SendAcknowledgePacket(Helpers.Bytes2String([state.Buffer[8]]));
                OnBattlEyeMessage(Helpers.Bytes2String(state.Buffer, 9, bytesRead - 9), 256);
            }
            else if (state.Buffer[7] == 0x01)
            {
                if (bytesRead > 9)
                {
                    if (state.Buffer[7] == 0x01 && state.Buffer[9] == 0x00)
                    {
                        if (state.Buffer[11] == 0)
                        {
                            state.PacketsTodo = state.Buffer[10];
                        }

                        if (state.PacketsTodo > 0)
                        {
                            state.Message.Append(Helpers.Bytes2String(state.Buffer, 12, bytesRead - 12));
                            state.PacketsTodo--;
                        }

                        if (state.PacketsTodo == 0)
                        {
                            OnBattlEyeMessage(state.Message.ToString(), state.Buffer[8]);
                            state.Message = new StringBuilder();
                            state.PacketsTodo = 0;
                        }
                    }
                    else
                    {
                        // Temporary fix to avoid infinite loops with multi-packet server messages
                        state.Message = new StringBuilder();
                        state.PacketsTodo = 0;

                        OnBattlEyeMessage(Helpers.Bytes2String(state.Buffer, 9, bytesRead - 9), state.Buffer[8]);
                    }
                }

                if (_packetQueue.ContainsKey(state.Buffer[8]) && state.PacketsTodo == 0)
                {
                    _packetQueue.Remove(state.Buffer[8]);
                }
            }

            _packetReceived = DateTime.Now;

            client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }
        catch
        {
            // do nothing
        }
    }

    private void OnBattlEyeMessage(string message, int id)
    {
        BattlEyeMessageReceived?.Invoke(new BattlEyeMessageEventArgs(message, id));
    }

    private void OnConnect(BattlEyeLoginCredentials loginDetails, BattlEyeConnectionResult connectionResult)
    {
        if (connectionResult == BattlEyeConnectionResult.ConnectionFailed || connectionResult == BattlEyeConnectionResult.InvalidLogin)
            Disconnect(null);

        BattlEyeConnected?.Invoke(new BattlEyeConnectEventArgs(loginDetails, connectionResult));
    }

    private void OnDisconnect(BattlEyeLoginCredentials loginDetails, BattlEyeDisconnectionType? disconnectionType)
    {
        BattlEyeDisconnected?.Invoke(new BattlEyeDisconnectEventArgs(loginDetails, disconnectionType));
    }

    public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
    public event BattlEyeConnectEventHandler BattlEyeConnected;
    public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
}

public class StateObject
{
    public Socket? WorkSocket;
    public const int BufferSize = 2048;
    public byte[] Buffer = new byte[BufferSize];
    public StringBuilder Message = new StringBuilder();
    public int PacketsTodo;
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.