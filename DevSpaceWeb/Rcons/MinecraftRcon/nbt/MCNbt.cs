using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace LibMCRcon.Nbt
{

    public enum NbtType
    {

        TAG_end = 0,
        TAG_byte = 1,
        TAG_short = 2,
        TAG_int = 3,
        TAG_long = 4,
        TAG_float = 5,
        TAG_double = 6,
        TAG_array_byte = 7,
        TAG_string = 8,
        TAG_list = 9,
        TAG_compound = 10,
        TAG_array_int = 11,
        TAG_array_long = 12
    }

    public interface INbtValues<T>
    {
        T tagvalues { get; set; }
    }

    public static class NbtWriter
    {
        public static void TagType(NbtType t, Stream s)
        {
            s.WriteByte((byte)t);
        }
        public static void TagByte(byte data, Stream s)
        {
            s.WriteByte(data);
        }
        public static void TagInt24(Int32 data, Stream s)
        {
            byte[] payload = new byte[3];
            byte[] intdata = new byte[4];

            intdata = BitConverter.GetBytes(data);
            Array.Copy(intdata, 0, payload, 0, 3);

            if (BitConverter.IsLittleEndian) Array.Reverse(payload);

            s.Write(payload, 0, 3);
        }
        public static void TagShort(Int16 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 2);
        }
        public static void TagShort(UInt16 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 2);
        }
        public static void TagInt(Int32 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 4);
        }
        public static void TagInt(UInt32 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 4);
        }
        public static void TagLong(Int64 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 8);
        }
        public static void TagLong(UInt64 data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 8);
        }
        public static void TagFloat(Single data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 4);
        }
        public static void TagDouble(Double data, Stream s)
        {
            byte[] payload = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(payload);
            s.Write(payload, 0, 8);
        }
        public static void TagString(String data, Stream s)
        {

            byte[] payload = Encoding.UTF8.GetBytes(data);
            TagShort((short)payload.Length, s);
            s.Write(payload, 0, payload.Length);
        }
        public static void TagRawString(String data, Stream s)
        {

            byte[] payload = Encoding.UTF8.GetBytes(data);
            s.Write(payload, 0, payload.Length);
        }
        public static void TagLongArray(Int64[] data, Stream s)
        {
            Int32 size = data.Length;

            TagInt(size, s);
            for (int x = 0; x < size; x++)
                TagLong(data[x], s);
        }
        public static void TagIntArray(Int32[] data, Stream s)
        {
            Int32 size = data.Length;

            TagInt(size, s);
            for (int x = 0; x < size; x++)
                TagInt(data[x], s);
        }
        public static void TagByteArray(byte[] data, Stream s)
        {
            Int32 size = data.Length;

            TagInt(size, s);
            for (int x = 0; x < size; x++)
                TagByte(data[x], s);
        }
        public static void WriteTag(NbtBase Nbt, Stream s)
        {
            TagType(Nbt.tagtype, s);
            TagString(Nbt.tagname, s);
            Nbt.WriteStream(s);
        }

    }
    public static class NbtReader
    {
        public static NbtType TagType(Stream s)
        {

            int rb = s.ReadByte();
            if (rb != -1)
                return (NbtType)rb;
            else
                return NbtType.TAG_end;

        }
        public static byte TagByte(Stream s)
        {
            return (byte)s.ReadByte();
        }
        public static Int32 TagInt24(Stream s)
        {
            byte[] payload = new byte[3];
            byte[] intdata = new byte[4];

            Array.Clear(intdata, 0, 4);

            if (s.Read(payload, 0, 3) == 3)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                Array.Copy(payload, 0, intdata, 0, 3);
                return BitConverter.ToInt32(intdata, 0);

            }

            return 0;
        }

        public static Int16 TagShort(Stream s)
        {
            byte[] payload = new byte[2];
            if (s.Read(payload, 0, 2) == 2)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToInt16(payload, 0);

            }

            return 0;
        }
        public static UInt16 TagUShort(Stream s)
        {
            byte[] payload = new byte[2];
            if (s.Read(payload, 0, 2) == 2)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToUInt16(payload, 0);

            }

            return 0;
        }

        public static Int32 TagInt(Stream s)
        {
            byte[] payload = new byte[4];
            if (s.Read(payload, 0, 4) == 4)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToInt32(payload, 0);

            }

            return 0;


        }
        public static UInt32 TagUInt(Stream s)
        {
            byte[] payload = new byte[4];
            if (s.Read(payload, 0, 4) == 4)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToUInt32(payload, 0);

            }

            return 0;


        }

        public static Int64 TagLong(Stream s)
        {
            byte[] payload = new byte[8];
            if (s.Read(payload, 0, 8) == 8)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToInt64(payload, 0);

            }

            return 0;
        }
        public static UInt64 TagULong(Stream s)
        {
            byte[] payload = new byte[8];
            if (s.Read(payload, 0, 8) == 8)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToUInt64(payload, 0);

            }

            return 0;
        }

        public static Single TagFloat(Stream s)
        {
            byte[] payload = new byte[4];
            if (s.Read(payload, 0, 4) == 4)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToSingle(payload, 0);

            }

            return Single.NaN;
        }
        public static Double TagDouble(Stream s)
        {
            byte[] payload = new byte[8];
            if (s.Read(payload, 0, 8) == 8)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(payload);
                return BitConverter.ToDouble(payload, 0);

            }

            return Double.NaN;
        }
        public static String TagString(Stream s)
        {

            Int16 slen = TagShort(s);

            byte[] payload = new byte[slen];
            if (s.Read(payload, 0, slen) == slen)
            {
                return Encoding.UTF8.GetString(payload);
            }

            return string.Empty;


        }
        public static Int64[] TagLongArray(Stream s)
        {

            Int64 size = TagInt(s);
            Int64[] intarr = new Int64[size];
            for (int x = 0; x < size; x++)
                intarr[x] = TagLong(s);

            return intarr;

        }
        public static Int32[] TagIntArray(Stream s)
        {

            Int32 size = TagInt(s);
            Int32[] intarr = new Int32[size];
            for (int x = 0; x < size; x++)
                intarr[x] = TagInt(s);

            return intarr;

        }
        public static Byte[] TagByteArray(Stream s)
        {

            Int32 size = TagInt(s);
            byte[] bytearr = new byte[size];
            for (int x = 0; x < size; x++)
                bytearr[x] = TagByte(s);

            return bytearr;
        }
        public static NbtBase ReadTag(Stream s)
        {
            NbtBase n = NbtBase.createtag(s);
            n.ReadStream(s);

            return n;
        }
    }

    public abstract class NbtBase
    {

        public long endpos { get; set; }

        public NbtType tagtype { get; set; }
        public string tagname { get; set; }
        public bool IsEnd { get { return tagtype == NbtType.TAG_end; } }

        public static NbtBase createtag(NbtType tag)
        {
            NbtBase n = null;


            switch (tag)
            {
                case NbtType.TAG_byte:
                    n = new NbtByte();
                    break;

                case NbtType.TAG_short:
                    n = new NbtShort();
                    break;

                case NbtType.TAG_int:
                    n = new NbtInt();
                    break;

                case NbtType.TAG_long:
                    n = new NbtLong();
                    break;

                case NbtType.TAG_string:
                    n = new NbtString();
                    break;

                case NbtType.TAG_float:
                    n = new NbtFloat();
                    break;

                case NbtType.TAG_double:
                    n = new NbtDouble();
                    break;

                case NbtType.TAG_array_byte:
                    n = new NbtByteArray();
                    break;

                case NbtType.TAG_array_int:
                    n = new NbtIntArray();
                    break;

                case NbtType.TAG_array_long:
                    n = new NbtLongArray();
                    break;

                case NbtType.TAG_compound:
                    n = new NbtCompound();
                    break;

                case NbtType.TAG_list:
                    n = new NbtList();
                    break;

                default:
                    throw new Exception("NBT Tag Invalid");

            }


            n.endpos = 0;

            return n;

        }
        public static NbtBase createtag(Stream s)
        {
            NbtType T = NbtReader.TagType(s);

            if (T == NbtType.TAG_end)
            {
                return new NbtEnd();
            }

            NbtBase n = createtag(T);
            n.tagname = NbtReader.TagString(s);
            return n;
        }
        public NbtBase this[string name]
        {
            get
            {
                if (name.Length == 0)
                    return null;

                if (name == tagname)
                    return this;

                NbtBase found = null;
                List<NbtBase> n;
                List<NbtBase> nn;

                switch (tagtype)
                {
                    case NbtType.TAG_compound:
                        n = ((NbtCompound)this).tagvalue;

                        break;
                    case NbtType.TAG_list:
                        n = ((NbtList)this).tagvalue;
                        break;
                    default:
                        return null;
                }

                found = n.Find(x => x.tagname == name);
                if (found != null)
                    return found;

                nn = n.FindAll(x => (x.tagtype == NbtType.TAG_compound || x.tagtype == NbtType.TAG_list));
                foreach (NbtBase x in nn)
                {
                    found = x[name];
                    if (found != null)
                        return found;
                }

                return null;
            }

        }

        public abstract void WriteStream(Stream s);
        public abstract void ReadStream(Stream s);



    }
    public abstract class NbtTag<T> : NbtBase
    {
        
        public virtual T tagvalue { get; set; }
        public abstract override void WriteStream(Stream s);
        public abstract override void ReadStream(Stream s);

    }

    public class NbtCompound : NbtTag<List<NbtBase>>
    {
        public NbtCompound()
        {
            tagtype = NbtType.TAG_compound;
            tagvalue = new List<NbtBase>();
            tagname = string.Empty;
        }
        public NbtBase this[int idx]
        {
            get
            {
                return tagvalue[idx];
            }
        }

        public override void WriteStream(Stream s)
        {
            foreach (NbtBase n in tagvalue)
            {
                NbtWriter.WriteTag(n, s);
                if (n.tagtype == NbtType.TAG_end)
                    break;
            }
        }
        public override void ReadStream(Stream s)
        {
            NbtBase n = createtag(s);
            while (n.tagtype != NbtType.TAG_end)
            {

                n.ReadStream(s);
                tagvalue.Add(n);
                n = createtag(s);
            }

        }
    }
    public class NbtList : NbtTag<List<NbtBase>>
    {

        public NbtType listtagtype { get; set; }

        public NbtList()
        {
            tagtype = NbtType.TAG_list;
            tagvalue = new List<NbtBase>();
            tagname = string.Empty;


        }


        public NbtBase this[int idx]
        {
            get
            {
                return tagvalue[idx];
            }
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagType(listtagtype, s);
            NbtWriter.TagInt(tagvalue.Count, s);
            foreach (NbtBase n in tagvalue)
            {
                n.WriteStream(s);
            }
        }
        public override void ReadStream(Stream s)
        {
            listtagtype = NbtReader.TagType(s);
            Int32 size = NbtReader.TagInt(s);
            for (int idx = 0; idx < size; idx++)
            {
                NbtBase n = createtag(listtagtype);
                n.ReadStream(s);
                tagvalue.Add(n);
            }
        }
    }
    public class NbtLongArray : NbtTag<Int64[]>
    {
        public NbtLongArray()
        {
            tagtype = NbtType.TAG_array_long;
            tagvalue = new Int64[1];
            tagname = string.Empty;

        }


        public int size { get { return tagvalue.GetLength(0); } }
        public override void WriteStream(Stream s)
        {
            NbtWriter.TagLongArray(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagLongArray(s);
        }
    }
    public class NbtIntArray : NbtTag<Int32[]>
    {
        public NbtIntArray()
        {
            tagtype = NbtType.TAG_array_int;
            tagvalue = new Int32[1];
            tagname = string.Empty;

        }


        public int size { get { return tagvalue.GetLength(0); } }
        public override void WriteStream(Stream s)
        {
            NbtWriter.TagIntArray(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagIntArray(s);
        }
    }
    public class NbtByteArray : NbtTag<byte[]>
    {
        public NbtByteArray()
        {
            tagtype = NbtType.TAG_array_byte;
            tagvalue = new byte[1];
            tagname = string.Empty;

        }

        public int size { get { return tagvalue.GetLength(0); } }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagByteArray(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagByteArray(s);
        }
    }
    public class NbtString : NbtTag<String>
    {
        public NbtString()
        {
            tagtype = NbtType.TAG_string;
            tagvalue = string.Empty;
            tagname = string.Empty;
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagString(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagString(s);
        }
    }
    public class NbtDouble : NbtTag<Double>
    {
        public NbtDouble()
        {
            tagtype = NbtType.TAG_double;
            tagvalue = Double.NaN;
            tagname = string.Empty;
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagDouble(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagDouble(s);
        }
    }
    public class NbtFloat : NbtTag<Single>
    {
        public NbtFloat()
        {
            tagtype = NbtType.TAG_float;
            tagvalue = Single.NaN;
            tagname = string.Empty;
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagFloat(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagFloat(s);
        }
    }
    public class NbtLong : NbtTag<Int64>
    {

        public NbtLong()
        {
            tagtype = NbtType.TAG_long;
            tagvalue = 0;
            tagname = string.Empty;
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagLong(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagLong(s);
        }
    }
    public class NbtInt : NbtTag<Int32>
    {
        public NbtInt()
        {
            tagtype = NbtType.TAG_int;
            tagvalue = 0;
            tagname = string.Empty;


        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagInt(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagInt(s);
        }
    }
    public class NbtShort : NbtTag<Int16>
    {

        public NbtShort()
        {
            tagtype = NbtType.TAG_short;
            tagvalue = 0;
            tagname = string.Empty;
        }

        public override void WriteStream(Stream s)
        {
            NbtWriter.TagShort(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagShort(s);
        }
    }
    public class NbtByte : NbtTag<byte>
    {


        public NbtByte()
        {
            tagtype = NbtType.TAG_byte;
            tagvalue = 0;
            tagname = string.Empty;
        }


        public override void WriteStream(Stream s)
        {
            NbtWriter.TagByte(tagvalue, s);
        }
        public override void ReadStream(Stream s)
        {
            tagvalue = NbtReader.TagByte(s);
        }
    }
    public class NbtEnd : NbtTag<NbtEnd>
    {
        public NbtEnd()
        {
            tagtype = NbtType.TAG_end;
            tagvalue = null;
            tagname = "END";

        }

        public override NbtEnd tagvalue
        {
            set
            {
                base.tagvalue = null;
            }
        }



        public override void WriteStream(Stream s)
        {
            s.WriteByte(0);
        }

        public override void ReadStream(Stream s)
        {
            return;
        }
    }

}



