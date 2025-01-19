using System;

using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;



//!Classes directly related to the minecraft server.
namespace LibMCRcon.RCon
{
    /// <summary>
    /// RCon packet reader/writter.
    /// </summary>
    public class RconPacket
    {
        Int32 size;
        Int32 packettype;
        String cmd;
        String response;
        bool isBadPacket;

        /// <summary>
        /// Constructor with default settings, empty packet.
        /// </summary>
        public RconPacket()
        {
            size = 10;
            SessionID = 0;
            packettype = 1;

        }

        /// <summary>
        /// Used internally to create a packet for transmission/reception.
        /// </summary>
        /// <param name="Command">Payload to send to server such as minecraft commands.</param>
        /// <param name="ServerPacket">As per RCon specification, what type of packet.</param>
        /// <param name="SessionID">Once generated, used throughout lifespan of connection.</param>
        private RconPacket(String Command, Int32 ServerPacket, Int32 SessionID)
        {
            if (Command.Length > 1446)
                cmd = Command.Substring(0, 1446);
            else
                cmd = Command;

            size = cmd.Length + 10;
            this.SessionID = SessionID;
            packettype = ServerPacket;

        }
        /// <summary>
        /// Helper function to create an Authorization packet, used in establishing connection to RCon handler on the minecraft server.
        /// </summary>
        /// <param name="Password">Password, plain text.</param>
        /// <param name="SessionID">Randomly generated integer, maintained throughout authorized connection lifetime.</param>
        /// <returns>A RCon packet ready to participate in Authentication handshake.</returns>
        public static RconPacket AuthPacket(String Password, Int32 SessionID)
        {
            return new RconPacket(Password, 3, SessionID);
        }
        /// <summary>
        /// Helper function to create a communication packet from client->server.
        /// </summary>
        /// <param name="Command">The payload of the packet, the minecraft server command in text.</param>
        /// <param name="SessionID">Maintains authorization and keeps all communication related.</param>
        /// <returns>A RCon packet ready to be transmitted and receive response.</returns>
        public static RconPacket CmdPacket(String Command, Int32 SessionID)
        {
            return new RconPacket(Command, 2, SessionID);
        }
        /// <summary>
        /// Helper function to fill a section of a byte array from using the entire source array.
        /// </summary>
        /// <param name="dest">Array to receive bytes.</param>
        /// <param name="source">Array bytes are from.</param>
        /// <param name="offset">Where to start the overlay of data in the destination array.</param>
        /// <param name="size">How many bytes to copy from the start of the source array into the destination. Must not be larger than the size of either array.</param>
        /// <returns>The dest array passed into the function.</returns>
        private byte[] fillByteArray(byte[] dest, byte[] source, int offset, int size)
        {


            if (dest.Length > offset + size)
                if (source.Length <= size)
                    for (int x = 0; x < size; x++)
                        dest[offset + x] = source[x];

            return dest;


        }
        /// <summary>
        /// Transmits the contents of the RCon packet - converts data into the packet format require by RCon and writes to the network stream.
        /// </summary>
        /// <param name="NS">Open network stream ready to receive data.  Will block until done.</param>
        public void SendToNetworkStream(NetworkStream NS)
        {
            isBadPacket = false;
            byte[] dataout = new byte[size + 4];

            dataout = fillByteArray(dataout, BitConverter.GetBytes(size), 0, 4);
            dataout = fillByteArray(dataout, BitConverter.GetBytes(SessionID), 4, 4);
            dataout = fillByteArray(dataout, BitConverter.GetBytes(packettype), 8, 4);
            dataout = fillByteArray(dataout, Encoding.ASCII.GetBytes(cmd), 12, cmd.Length);
            dataout[size + 2] = 0;
            dataout[size + 3] = 0;

            try
            {
                NS.Write(dataout, 0, dataout.Length);
            }
            catch (Exception)
            {
                isBadPacket = true;
            }
        }
        public async Task SendToNetworkStreamAsync(NetworkStream NS)
        {
            isBadPacket = false;
            byte[] dataout = new byte[size + 4];

            dataout = fillByteArray(dataout, BitConverter.GetBytes(size), 0, 4);
            dataout = fillByteArray(dataout, BitConverter.GetBytes(SessionID), 4, 4);
            dataout = fillByteArray(dataout, BitConverter.GetBytes(packettype), 8, 4);
            dataout = fillByteArray(dataout, Encoding.ASCII.GetBytes(cmd), 12, cmd.Length);
            dataout[size + 2] = 0;
            dataout[size + 3] = 0;

            try
            {
                await NS.WriteAsync(dataout, 0, dataout.Length);

            }
            catch (Exception)
            {
                isBadPacket = true;
            }
        }
        /// <summary>
        /// Read from the network stream the next valid RCon packet. Will block until completed or an error detected.
        /// </summary>
        /// <param name="NS">Network stream ready for reception of data.</param>
        public void ReadFromNetworkSteam(NetworkStream NS)
        {
            byte[] s = new byte[4];
            Int16 endzeros;

            isBadPacket = false;

            bool FullRead(byte[] Buffer, int TotalBytes)
            {

                var idx = 0;
                var count = TotalBytes;

                var rx = NS.Read(Buffer, idx, TotalBytes);
                
                while (rx < count)
                {

                    if (rx == 0)
                        return false;

                    idx += rx;
                    count -= rx;

                    rx = NS.Read(Buffer, idx, count);
                }

                return true;
            }

            try
            {


                if (FullRead(s, 4))
                {
                    size = BitConverter.ToInt32(s, 0);

                    if (size >= 10 && size < 4096)
                    {
                        byte[] data = new byte[size];
                        if (FullRead(data, size))
                        {

                            SessionID = BitConverter.ToInt32(data, 0);
                            packettype = BitConverter.ToInt32(data, 4);

                            if ((size - 10) > 0)
                            {

                                response = Encoding.ASCII.GetString(data, 8, size - 10);

                            }

                            endzeros = BitConverter.ToInt16(data, size - 2);


                            if (endzeros != 0)
                            {
                                //frame is bad - always ends in 2 zeros..

                                isBadPacket = true;
                            }
                        }
                        else
                            isBadPacket = true;
                    }
                    else

                        isBadPacket = true;



                }
                else
                    isBadPacket = true;
            }

            catch (Exception e)
            {
                isBadPacket = true;
                response = e.Message;

            }


        }

       

        public async Task ReadFromNetworkSteamAsync(NetworkStream NS)
        {
            byte[] s = new byte[4];
            Int16 endzeros;

            isBadPacket = false;

            async Task<bool> FullReadAsync(byte[] Buffer, int TotalBytes)
            {

                var idx = 0;
                var count = TotalBytes;

                var rx = await NS.ReadAsync(Buffer, idx, TotalBytes);

                while (rx < count)
                {

                    if (rx == 0)
                        return false;

                    idx += rx;
                    count -= rx;

                    rx = await NS.ReadAsync(Buffer, idx, count);
                }

                return true;
            }

            try
            {
                if (await FullReadAsync(s, 4))
                {

                    size = BitConverter.ToInt32(s, 0);

                    if (size >= 10 && size < 4096)
                    {
                        byte[] data = new byte[size];

                        if (await FullReadAsync(data, size))
                        {



                            SessionID = BitConverter.ToInt32(data, 0);
                            packettype = BitConverter.ToInt32(data, 4);

                            if ((size - 10) > 0)
                                response = Encoding.ASCII.GetString(data, 8, size - 10);

                            endzeros = BitConverter.ToInt16(data, size - 2);


                            if (endzeros != 0)
                                isBadPacket = true;
                        }
                        else
                            isBadPacket = true;
                    }
                    else
                        isBadPacket = true;



                }
                else
                    isBadPacket = true;
            }
            catch (Exception e)
            {
                isBadPacket = true;
                response = e.Message;

            }


        }

        /// <summary>
        /// Returns the current response stored.
        /// </summary>
        public String Response { get { return response; } }
        /// <summary>
        /// Generally bad packets/communication won't automatically close the connection.  The packet is marked bad if something wrong happens.
        /// </summary>
        public bool IsBadPacket { get { return isBadPacket; } }
        /// <summary>
        /// The RCon packet type.
        /// </summary>
        public Int32 ServerType { get { return packettype; } }
        /// <summary>
        /// The session id for the packet.
        /// </summary>
        public Int32 SessionID { get; private set; }
        /// <summary>
        /// The payload of an RCon packet, in this case expected to be a minecraft command.
        /// </summary>
        public String Cmd { get { return cmd; } }

    }

}
