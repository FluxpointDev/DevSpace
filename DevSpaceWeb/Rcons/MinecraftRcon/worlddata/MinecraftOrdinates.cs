
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace LibMCRcon.WorldData
{

    public static class MinecraftOrdinates
    {
        public static Voxel Region() { return new Voxel(int.MaxValue, 512) { WorldY = 0, WorldX = 0, WorldZ = 0 }; }
        public static Voxel Region(int y, int x, int z) { return new Voxel(int.MaxValue, 512) { WorldY = y, WorldX = x, WorldZ = z }; }
        public static Voxel Region(Voxel Voxel) { return new Voxel(int.MaxValue, 512) { WorldY = Voxel.WorldY, WorldX = Voxel.WorldX, WorldZ = Voxel.WorldZ }; }

        public static Voxel World(int RegionsPerWorld) { return new Voxel(int.MaxValue, 512 * RegionsPerWorld) { WorldY = 0, WorldX = 0, WorldZ = 0 }; }
        public static Voxel World(int RegionsPerWorld, int y, int x, int z) { return new Voxel(int.MaxValue, 512 * RegionsPerWorld) { WorldY = y, WorldX = x, WorldZ = z }; }
        public static Voxel World(int RegionsPerWorld, Voxel Voxel) { return new Voxel(int.MaxValue, 512 * RegionsPerWorld) { WorldY = Voxel.WorldY, WorldX = Voxel.WorldX, WorldZ = Voxel.WorldZ }; }

        public static Voxel Chunk() { return new Voxel(16, 16) { WorldY = 0, WorldX = 0, WorldZ = 0 }; }
        public static Voxel Chunk(int y, int x, int z) { return new Voxel(16, 16) { WorldY = y, WorldX = x, WorldZ = z }; }
        public static Voxel Chunk(Voxel Voxel) { return Voxel.OffsetVoxel(16, 16); }

        public static int ChunkIdx(Voxel Chunk) { return (Chunk.Zs * 32) + Chunk.Xs; }
        public static int ChunkZXidx(Voxel Chunk) { return (Chunk.Zo * 16) + Chunk.Xo; }
        public static int ChunkBlockPos(Voxel Chunk) { return (Chunk.Yo * 16 * 16) + (Chunk.Zo * 16) + Chunk.Xo; }

        public static int[] WorldMapRenderBoundries(string[] ImgNames)
        {
            List<string> ImgNameList = new List<string>(ImgNames);

            int x = 0;
            int z = 0;

            int maxX = int.MinValue;
            int maxZ = int.MinValue;

            foreach (string f in ImgNameList)
            {

                string[] ParseData = f.Split('.');

                //0 - Type of thumb (WorldTopo64, WorldTile64, WorldTopo32, etc)
                //1 - World Thumb coordinate - X 
                //2 - World Thumb coordinate - Y
                //3 - left most X
                //4 - right most X
                //5 - left most z
                //6 - right most z
                x = 0; z = 0;


                if (int.TryParse(ParseData[1], out x))
                    if (int.TryParse(ParseData[2], out z))
                    {


                        if (x > maxX) maxX = x;
                        if (z > maxZ) maxZ = z;

                    }
            }

            string w = ImgNameList.Find(xx => { string[] s = xx.Split('.'); if (s[1] == "0" && s[2] == "0") return true; else return false; });
            Voxel[] ULV = WorldMapBoundriesFromImg(w);

            w = ImgNameList.Find(xx => { string[] s = xx.Split('.'); if (s[1] == maxX.ToString() && s[2] == maxZ.ToString()) return true; else return false; });
            Voxel[] LRV = WorldMapBoundriesFromImg(w);

            int A = Math.Abs(ULV[0].WorldX - LRV[1].WorldX);
            int B = Math.Abs(ULV[0].WorldZ - LRV[1].WorldZ);

            return new int[] { maxX, maxZ, ULV[0].WorldX, ULV[0].WorldZ, LRV[1].WorldX, LRV[1].WorldZ, A, B };

            //0 - max thumb x 
            //1 - max thumb z (y)
            //2 - upper left minecraft block X
            //3 - upper left minecraft block z
            //4 - lower right minecraft block x
            //5 - lower right minecraft block z
            //6 - total x-axis blocks
            //7 - total z-axis blocks
        }
        public static int[] WorldMapRenderBoundries(string WorldThumbPrefix, string ImgsPath)
        {

            DirectoryInfo di = new DirectoryInfo(ImgsPath);
            int x = 0;
            int z = 0;

            int maxX = int.MinValue;
            int maxZ = int.MinValue;

            foreach (FileInfo f in di.GetFiles(WorldThumbPrefix + ".*.png"))
            {

                string[] ParseData = Path.GetFileNameWithoutExtension(f.Name).Split('.');

                //0 - Type of thumb (WorldTopo64, WorldTile64, WorldTopo32, etc)
                //1 - World Thumb coordinate - X 
                //2 - World Thumb coordinate - Y
                //3 - left most X
                //4 - right most X
                //5 - left most z
                //6 - right most z
                x = 0; z = 0;
                

                if (int.TryParse(ParseData[1], out x))
                    if (int.TryParse(ParseData[2], out z))
                    {


                        if (x > maxX) maxX = x;
                        if (z > maxZ) maxZ = z;

                    }
            }

            FileInfo[] Thumb = di.GetFiles(WorldThumbPrefix + ".0.0.*.png");
            Voxel[] ULV = WorldMapBoundriesFromImg(Thumb[0].Name);

            Thumb = di.GetFiles(string.Format("{0}.{1}.{2}.*.png", WorldThumbPrefix, maxX, maxZ));
            Voxel[] LRV = WorldMapBoundriesFromImg(Thumb[0].Name);

            int A = Math.Abs(ULV[0].WorldX - LRV[1].WorldX);
            int B = Math.Abs(ULV[0].WorldZ - LRV[1].WorldZ);

            return new int[] { maxX, maxZ, ULV[0].WorldX, ULV[0].WorldZ, LRV[1].WorldX, LRV[1].WorldZ, A, B };
            
            //0 - max thumb x 
            //1 - max thumb z (y)
            //2 - upper left minecraft block X
            //3 - upper left minecraft block z
            //4 - lower right minecraft block x
            //5 - lower right minecraft block z
            //6 - total x-axis blocks
            //7 - total z-axis blocks


        }
        public static Voxel[] WorldMapBoundriesFromImg(string WorldMapImg)
        {

            Voxel V1 = MinecraftOrdinates.Region();
            Voxel V2 = MinecraftOrdinates.Region();


            string[] ParseData = WorldMapImg.Split('.');
            //0 - Type of thumb (WorldTopo64, WorldTile64, WorldTopo32, etc)
            //1 - World Thumb coordinate - X 
            //2 - World Thumb coordinate - Y
            //3 - left most x
            //4 - right most x
            //5 - left most z
            //6 - right most z

            int x1 = 0;
            int x2 = 0;
            int z1 = 0;
            int z2 = 0;

            if (int.TryParse(ParseData[3], out x1))
                if (int.TryParse(ParseData[4], out x2))
                    if(int.TryParse(ParseData[5], out z1))
                        if (int.TryParse(ParseData[6], out z2))
                        {
                            V1.SetSegmentOffset(x1, z1, 0, 0);
                            V2.SetSegmentOffset(x2, z2, 511, 511);
                        }

            return new Voxel[] { V1, V2 };
        }
        public static Voxel[] WorldMapBoundries(string Path)
        {

            int x1 = int.MaxValue;
            int z1 = int.MaxValue;
            int x2 = int.MinValue;
            int z2 = int.MinValue;

            

            DirectoryInfo di = new DirectoryInfo(Path);
            FileInfo[] Files = di.GetFiles("*.mca");

            foreach (FileInfo f in Files)
            {

                string[] v = f.Name.Split('.');

                if (v.Length > 2)
                {
                    int x = 0;
                    if (int.TryParse(v[1], out x) == true)
                    {
                        int z = 0;
                        if (int.TryParse(v[2], out z) == true)
                        {

                            if (x < x1) x1 = x;
                            if (x > x2) x2 = x;

                            if (z < z1) z1 = z;
                            if (z > z2) z2 = z;

                        }
                    }
                }
            }

            Voxel V1 = MinecraftOrdinates.Region();
            Voxel V2 = MinecraftOrdinates.Region();

            V1.SetSegmentOffset(x1, z1, 0, 0);
            V2.SetSegmentOffset(x2, z2, 511, 511);

            return new Voxel[] { V1, V2 };

        }


    }
}