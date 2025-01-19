using LibMCRcon.Nbt;
using Org.BouncyCastle.Utilities.Zlib;


namespace LibMCRcon.Network
{
    public enum mcProtoStates { HandShake, Play }
    public enum mcProtoTargets { Server, Client }

    public abstract class mcProto
    {
        public abstract void SendPacket(Stream s);
        public abstract void ReceivePacket();

        public mcProto() { }
        public mcProto(mcProtoPacket Packet) { this.Packet = Packet; }
        public mcProtoPacket Packet { get; set; }

    }
    public class mcProtoPacket
    {

        public int PacketLength { get; internal set; }
        public int DataLength { get; internal set; }

        public int PacketId { get; set; }
        public byte[] Data { get; set; }

        public bool DataRead { get; internal set; }
        public bool Compression { get; set; }

        public mcProtoPacket()
        {
            PacketLength = 0;
            PacketId = 0;
            DataLength = 0;
            Compression = false;
            DataRead = false;


        }
        public mcProtoPacket(bool UseCompression)
        {
            Compression = UseCompression;
        }
        public mcProtoPacket(bool UseCompression, mcProto PacketHandler)
        {
            Compression = UseCompression;
            Packet = PacketHandler;
        }

        public void ReadPacket(Stream s)
        {
            int slen = 0;
            byte[] rdata;

            VarInt viPacketLength = new VarInt(s);
            PacketLength = VarintBitConverter.ToInt32(viPacketLength.VarIntData);

            VarInt viPacketID;

            if (Compression == false)
            {

                viPacketID = new VarInt(s);
                PacketId = VarintBitConverter.ToInt32(viPacketID.VarIntData);

                DataLength = PacketLength - viPacketID.Length;
                Data = new byte[DataLength];


                if (s.Read(Data, 0, DataLength) == DataLength)
                    DataRead = true;
                else
                    DataRead = false;
            }
            else
            {

                VarInt viDataLength = new VarInt(s);
                DataLength = VarintBitConverter.ToInt32(viDataLength.VarIntData);


                if (DataLength == 0)
                {
                    //No compression
                    viPacketID = new VarInt(s);
                    PacketId = VarintBitConverter.ToInt32(viPacketID.VarIntData);
                    DataLength = PacketLength - viDataLength.Length - viPacketID.Length;
                    Data = new byte[DataLength];

                    if (s.Read(Data, 0, DataLength) == DataLength)
                        DataRead = true;
                    else
                        DataRead = false;

                }
                else
                {
                    Data = new byte[DataLength];
                    slen = PacketLength - viDataLength.Length;
                    rdata = new byte[slen];

                    if (s.Read(rdata, 0, slen) == slen)
                        DataRead = true;
                    else
                        DataRead = false;

                    if (DataRead == true)
                    {

                        MemoryStream ms;

                        ms = new MemoryStream(rdata);
                        ZInputStream zlib = new ZInputStream(ms);

                        if (zlib.Read(Data, 0, DataLength) == DataLength)
                            DataRead = true;
                        else
                            DataRead = false;

                        zlib.Close();

                        ms = new MemoryStream(Data);

                        viPacketID = new VarInt(ms);
                        PacketId = VarintBitConverter.ToInt32(viPacketID.VarIntData);

                        slen = DataLength - viPacketID.Length;


                        if (ms.Read(Data, 0, slen) == slen)
                            DataRead = true;
                        else
                            DataRead = false;

                        ms.Close();

                    }
                }

            }


        }
        public void WritePacket(Stream s)
        {
            VarInt viPacketID = new VarInt(PacketId);

            if (Compression == false)
            {

                PacketLength = DataLength + viPacketID.Length;
                VarInt viPacketLength = new VarInt(PacketLength);

                s.Write(viPacketID.VarIntData, 0, viPacketID.Length);
                s.Write(viPacketLength.VarIntData, 0, viPacketLength.Length);
                s.Write(Data, 0, DataLength);

            }
            else
            {


                MemoryStream ms = new MemoryStream();
                ZOutputStream zo = new ZOutputStream(ms);


                zo.Write(viPacketID.VarIntData, 0, viPacketID.Length);
                zo.Write(Data, 0, Data.Length);

                DataLength = Data.Length;

                int slen = (int)zo.Position;
                ms.Position = 0;
                zo.Close();

                VarInt viDataLength = new VarInt(DataLength);
                PacketLength = slen + viDataLength.Length;

                VarInt viPacketLength = new VarInt(PacketLength);

                s.Write(viPacketLength.VarIntData, 0, viPacketLength.Length);
                s.Write(viDataLength.VarIntData, 0, viDataLength.Length);
                ms.CopyTo(s);
                ms.Close();

            }


        }

        public mcProto Packet { get; set; }

    }

    public class mcProtoHandshake : mcProto
    {

        public int ProtocolVersion { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }
        public int NextState { get; set; }

        public mcProtoHandshake()
        {

            ProtocolVersion = 47;
            Port = 25565;

        }
        public mcProtoHandshake(mcProtoPacket Packet) : this()
        {
            this.Packet = Packet;
        }

        public override void SendPacket(Stream s)
        {

            VarInt vi = new VarInt();

            MemoryStream ms = new MemoryStream();

            vi.SetValue(ProtocolVersion);
            ms.Write(vi.VarIntData, 0, vi.Length);

            vi.SetValue(Address.Length);
            ms.Write(vi.VarIntData, 0, vi.Length);

            NbtWriter.TagRawString(Address, ms);
            NbtWriter.TagShort(Port, ms);

            vi.SetValue(NextState);
            ms.Write(vi.VarIntData, 0, vi.Length);

            Packet.DataLength = (int)ms.Position;
            Packet.Data = ms.ToArray();

            ms.Close();

            Packet.WritePacket(s);

        }

        public override void ReceivePacket()
        {
            MemoryStream ms = new MemoryStream(Packet.Data);
            VarInt vi = new VarInt(ms);

            ProtocolVersion = VarintBitConverter.ToInt32(vi.VarIntData);
            vi.SetValue(ms);



        }



    }



    public class mcProtoKeepAlive : mcProtoPacket
    {
        public int KeepAlive { get; set; }
        public void SendPacket(Stream s)
        {

            Data = VarintBitConverter.GetVarintBytes(KeepAlive);

            WritePacket(s);

        }
    }

    public class VarInt
    {

        public int Length { get { return VarIntData.Length; } }
        public byte[] VarIntData { get; private set; }

        public VarInt()
        {
            VarIntData = new byte[0];
        }
        public VarInt(Stream s)
        {
            VarIntData = VarIntFromStream(s);
        }
        public VarInt(Byte value)
        {
            SetValue(value);
        }
        public VarInt(Int16 value)
        {
            SetValue(value);
        }
        public VarInt(Int32 value)
        {
            SetValue(value);
        }
        public VarInt(Int64 value)
        {
            SetValue(value);
        }
        public VarInt(UInt16 value)
        {
            SetValue(value);
        }
        public VarInt(UInt32 value)
        {
            SetValue(value);
        }
        public VarInt(UInt64 value)
        {
            SetValue(value);
        }

        public void SetValue(Byte value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(Int16 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(Int32 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(Int64 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(UInt16 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(UInt32 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(UInt64 value)
        {
            VarIntData = VarintBitConverter.GetVarintBytes(value);
        }
        public void SetValue(Stream s)
        {
            VarIntData = VarIntFromStream(s);
        }

        public byte ToByte { get { return VarintBitConverter.ToByte(VarIntData); } }

        public short ToInt16 { get { return VarintBitConverter.ToInt16(VarIntData); } }
        public int ToInt32 { get { return VarintBitConverter.ToInt32(VarIntData); } }
        public long ToInt64 { get { return VarintBitConverter.ToInt64(VarIntData); } }

        public ushort ToUInt16 { get { return VarintBitConverter.ToUInt16(VarIntData); } }
        public uint ToUInt32 { get { return VarintBitConverter.ToUInt32(VarIntData); } }
        public ulong ToUInt64 { get { return VarintBitConverter.ToUInt64(VarIntData); } }

        public static byte[] VarIntFromStream(Stream s)
        {
            List<byte> data = new List<byte>(10);
            for (int x = 0; x < 10; x++)
            {
                int sdata = s.ReadByte();
                if (sdata == -1)
                    break;
                else
                    data.Add((byte)sdata);


                if ((sdata & 0x80) != 0x80)
                {
                    break;
                }

            }

            if (data.Count > 0)
            {
                return data.ToArray();
            }
            else
                return new byte[0];
        }



    }
    public class VarintBitConverter
    {

        ///VarintBitConverter:
        ///https://github.com/topas/VarintBitConverter 
        ///Copyright (c) 2011 Tomas Pastorek, Ixone.cz. All rights reserved.
        /// <summary>
        /// Returns the specified byte value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">Byte value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(byte value)
        {
            return GetVarintBytes((ulong)value);
        }

        /// <summary>
        /// Returns the specified 16-bit signed value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">16-bit signed value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(short value)
        {
            var zigzag = EncodeZigZag(value, 16);
            return GetVarintBytes((ulong)zigzag);
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">16-bit unsigned value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(ushort value)
        {
            return GetVarintBytes((ulong)value);
        }

        /// <summary>
        /// Returns the specified 32-bit signed value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">32-bit signed value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(int value)
        {
            var zigzag = EncodeZigZag(value, 32);
            return GetVarintBytes((ulong)zigzag);
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">32-bit unsigned value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(uint value)
        {
            return GetVarintBytes((ulong)value);
        }

        /// <summary>
        /// Returns the specified 64-bit signed value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">64-bit signed value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(long value)
        {
            var zigzag = EncodeZigZag(value, 64);
            return GetVarintBytes((ulong)zigzag);
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned value as varint encoded array of bytes.   
        /// </summary>
        /// <param name="value">64-bit unsigned value</param>
        /// <returns>Varint array of bytes.</returns>
        public static byte[] GetVarintBytes(ulong value)
        {
            var buffer = new List<byte>(10);
            do
            {
                var byteVal = value & 0x7f;
                value >>= 7;

                if (value != 0)
                {
                    byteVal |= 0x80;
                }

                buffer.Add((byte)byteVal);

            } while (value != 0);

            return buffer.ToArray();
        }

        /// <summary>
        /// Returns byte value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>Byte value</returns>
        public static byte ToByte(byte[] bytes)
        {
            return (byte)ToTarget(bytes, 8);
        }

        /// <summary>
        /// Returns 16-bit signed value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>16-bit signed value</returns>
        public static short ToInt16(byte[] bytes)
        {
            var zigzag = ToTarget(bytes, 16);
            return (short)DecodeZigZag(zigzag);
        }

        /// <summary>
        /// Returns 16-bit usigned value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>16-bit usigned value</returns>
        public static ushort ToUInt16(byte[] bytes)
        {
            return (ushort)ToTarget(bytes, 16);
        }

        /// <summary>
        /// Returns 32-bit signed value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>32-bit signed value</returns>
        public static int ToInt32(byte[] bytes)
        {
            var zigzag = ToTarget(bytes, 32);
            return (int)DecodeZigZag(zigzag);
        }

        /// <summary>
        /// Returns 32-bit unsigned value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>32-bit unsigned value</returns>
        public static uint ToUInt32(byte[] bytes)
        {
            return (uint)ToTarget(bytes, 32);
        }

        /// <summary>
        /// Returns 64-bit signed value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>64-bit signed value</returns>
        public static long ToInt64(byte[] bytes)
        {
            var zigzag = ToTarget(bytes, 64);
            return DecodeZigZag(zigzag);
        }

        /// <summary>
        /// Returns 64-bit unsigned value from varint encoded array of bytes.
        /// </summary>
        /// <param name="bytes">Varint encoded array of bytes.</param>
        /// <returns>64-bit unsigned value</returns>
        public static ulong ToUInt64(byte[] bytes)
        {
            return ToTarget(bytes, 64);
        }

        public static long EncodeZigZag(long value, int bitLength)
        {
            return (value << 1) ^ (value >> (bitLength - 1));
        }

        public static long DecodeZigZag(ulong value)
        {
            if ((value & 0x1) == 0x1)
            {
                return (-1 * ((long)(value >> 1) + 1));
            }

            return (long)(value >> 1);
        }

        public static ulong ToTarget(byte[] bytes, int sizeBites)
        {
            int shift = 0;
            ulong result = 0;

            foreach (ulong byteValue in bytes)
            {
                ulong tmp = byteValue & 0x7f;
                result |= tmp << shift;

                if (shift > sizeBites)
                {
                    throw new ArgumentOutOfRangeException("bytes", "Byte array is too large.");
                }

                if ((byteValue & 0x80) != 0x80)
                {
                    return result;
                }

                shift += 7;
            }

            throw new ArgumentException("Cannot decode varint from byte array.", "bytes");
        }



    }


}
