
using System;
using System.IO.Compression;
using System.IO;


using LibMCRcon.Nbt;

namespace LibMCRcon.WorldData
{
    public class ChunkMCA
    {
        public int chunksectorsize { get; protected set; }
        public DateTime timestamp { get; protected set; }

        public byte[] chunkdata { get; private set; }
        public bool chunkloaded { get; private set; }
        public bool chunkexists { get; private set; }

        public ChunkMCA()
        {

            chunksectorsize = 0;
            chunkdata = new byte[4096];
            chunkexists = false;


        }
        public ChunkMCA(int ChunkOffset, int ChunkSectorsSize, Stream readstream)
        {


            chunksectorsize = ChunkSectorsSize;
            chunkdata = new byte[chunksectorsize];

            if (ChunkOffset > 0)
            {
                chunkexists = true;


                if (readstream.Read(chunkdata, 0, chunksectorsize) == chunksectorsize)

                    chunkloaded = true;

                else
                    chunkloaded = false;
            }
            else
            {
                chunkexists = false;
                chunkloaded = false;
            }




        }
        
        public NbtCompound chunkNBT
        {
            get
            {
                if (chunkexists == false)
                    return null;

                MemoryStream ms;


                byte[] int32data = new byte[4];
                byte[] int16data = new byte[2];

                // byte[] chunkraw = new byte[0xfa000];

                Array.Copy(chunkdata, 0, int32data, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(int32data);

                Array.Copy(chunkdata, 5, int16data, 0, 2);
                if (BitConverter.IsLittleEndian) Array.Reverse(int16data);

                int zcomp = chunkdata[4];
                int chunksize = BitConverter.ToInt32(int32data, 0);
                int zcomphdr = BitConverter.ToInt16(int16data, 0);

                //ms = new MemoryStream(chunkdata, 7, chunksize - 3);
                ms = new MemoryStream(chunkdata);
                ms.Seek(7, SeekOrigin.Begin);

                DeflateStream zlib = new DeflateStream(ms, CompressionMode.Decompress);

                NbtBase nbt = NbtReader.ReadTag(zlib);
                zlib.Close();

                return (NbtCompound)nbt;



            }
        }
    }
}