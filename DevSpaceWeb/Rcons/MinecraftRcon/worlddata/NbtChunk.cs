
using System;
using System.Collections.Generic;


using LibMCRcon.Nbt;

namespace LibMCRcon.WorldData
{
    public class NbtChunk
    {


        public NbtInt xPos { get; private set; }
        public NbtInt zPos { get; private set; }
        public NbtLong lastUpdate { get; private set; }
        public NbtByte lightPopulated { get; private set; }
        public NbtByte terrainPopulated { get; private set; }
        public NbtByte V { get; private set; }
        public NbtLong inhabitedTime { get; private set; }
        public NbtIntArray biomes { get; private set; }
        public NbtCompound heightMap { get; private set; }
        public NbtList sections { get; private set; }
        public NbtList entities { get; private set; }
        public NbtList tileEntities { get; private set; }
        public NbtList tileTicks { get; private set; }

        public bool IsLoaded { get; private set; }

       
        private void LoadChunkData(NbtCompound chunkdata)
        {
            xPos = (NbtInt)chunkdata["xPos"];
            zPos = (NbtInt)chunkdata["zPos"];
            lastUpdate = (NbtLong)chunkdata["LastUpdate"];
            lightPopulated = (NbtByte)chunkdata["LighPopulated"];
            terrainPopulated = (NbtByte)chunkdata["TerrainPopulated"];
            V = (NbtByte)chunkdata["V"];
            inhabitedTime = (NbtLong)chunkdata["InhabitedTime"];

            LoadForSurvey(chunkdata);
            LoadForEntities(chunkdata);

            tileTicks = (NbtList)chunkdata["TileTicks"];
        }
        private void LoadForSurvey(NbtCompound chunkdata)
        {
            sections = (NbtList)chunkdata["Sections"];
            if (sections != null)
            {
                var c = chunkdata["Biomes"];
                if (c.tagtype == NbtType.TAG_array_byte)
                {

                }

                biomes = (NbtIntArray)chunkdata["Biomes"];
                heightMap = (NbtCompound)chunkdata["Heightmaps"];
            }
            else
            {

            }
        }
        private void LoadForEntities(NbtCompound chunkdata)
        {
            entities = (NbtList)chunkdata["Entities"];
            tileEntities = (NbtList)chunkdata["TileEntities"];
        }

        public NbtChunk() { }

        public NbtChunk(NbtCompound chunkdata)
            : this()
        {
            IsLoaded = false;

            if (chunkdata != null)
            {
                LoadChunkData(chunkdata);
                IsLoaded = true;
            }

        }

        public int ChunkX { get { return xPos.tagvalue; } }
        public int ChunkZ { get { return zPos.tagvalue; } }

        public bool HasTerrain { get { return terrainPopulated != null ? terrainPopulated.tagvalue == 1 ? true : false : false; } }

        public int Height(int x, int z)
        {
            if (heightMap == null)
                return 255;
            return 0;
           // return heightMap.tagvalue[((z & 0x000F) * 16) + (x & 0x000F)];
        }
        public int Height(int ChunkZXIdx)
        {
            if (heightMap == null)
                return 255;
            return 0;

            //return heightMap.tagvalue[ChunkZXIdx];
        }

        public int[] HeightData
        {
            get
            {
                return new int[] { };

                //return heightMap.tagvalue;
            }
        }

        public int Biome(int x, int z)
        {
            if (biomes == null)
                return -1;

            return biomes.tagvalue[((z & 0x000F) * 16) + (x & 0x000F)];

        }
        public int Biome(int ChunkZXIdx)
        {
            if (biomes == null)
                return -1;
            return biomes.tagvalue[ChunkZXIdx];

        }

        public int[] BiomeData
        {
            get
            {
                return biomes.tagvalue;
            }
        }

        public NbtCompound Section(Int32 y)
        {
            if (sections == null)
                return null;
           
            try
            {
                return (NbtCompound)sections.tagvalue.Find(x => (((NbtByte)(((NbtCompound)x)["Y"])).tagvalue == y));
            }
            catch (Exception)
            {
                return null;
            }




        }

        public NbtChunkSection BlockSection(Int32 y) { return new NbtChunkSection(Section(y)); }
        public List<NbtChunkSection> BlockSections()
        {

            if (sections == null)
            {
                return new List<NbtChunkSection>();
            }

            List<NbtChunkSection> blocks = new List<NbtChunkSection>(sections.tagvalue.Count);

            foreach (NbtCompound section in sections.tagvalue)
                blocks.Add(new NbtChunkSection(section));



            return blocks;
        }

    }
}