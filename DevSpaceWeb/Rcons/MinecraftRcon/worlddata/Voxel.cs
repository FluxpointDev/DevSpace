using System;
using System.Text;
using System.Collections.Generic;


namespace LibMCRcon.WorldData
{

    public class Voxel
    {
       

        public static int Segment(int Size, int Ordinate)
        {
            return Ordinate < 0 ? (((Ordinate + 1) / Size) - 1) : (Ordinate / Size);
        }
        public static int Offset(int Size, int Ordinate)
        {
            return Ordinate < 0 ? -((Size * (((Ordinate + 1) / Size) - 1)) - Ordinate) : Ordinate - (Size * (Ordinate / Size));
        }
        public static int Ordinate(int Size, int Segment, int Offset)
        {

            if (Segment < 0)
                //return -(((Segment * -1) * Size) - Offset);
                return -((-Segment * Size) - Offset);
            else
                return (Segment * Size) + Offset;
        }

        public static int ScaledSegment( int RegionsPerWorld, int Ordinate, int Size = 512)
        {
            return Ordinate < 0 ? (((Ordinate + 1) / (Size * RegionsPerWorld)) - 1) : (Ordinate / (Size * RegionsPerWorld));
        }
        public static int ScaledOffset(int RegionsPerWorld, int Ordinate, int Size = 512)
        {
            int s = Size * RegionsPerWorld;
            return (Ordinate < 0 ? -((s * (((Ordinate + 1) / s) - 1)) - Ordinate) : Ordinate - (s * Segment(s, Ordinate))) / RegionsPerWorld;
        }
        public static int ScaledOrdinate(int RegionsPerWorld, int Segment, int Offset, int Size = 512)
        {
            int s = Size * RegionsPerWorld;
            
            if (Segment < 0)
                //return -(((Segment * -1) * Size) - Offset);
                return -((-Segment * s) - (Offset * RegionsPerWorld));
            else
                return (Segment * s) + (Offset * RegionsPerWorld);
        }




        public Voxel() : this(int.MaxValue, 512) { WorldY = 65; WorldX = 0; WorldZ = 0; }
        public Voxel(int YSize, int XZSize)
        {
            Yz = YSize;
            Sz = XZSize;

            V = new int[3] { 0, 0, 0 };
            S = new int[3] { 0, 0, 0 };
            
            IsValid = true;
        }
        public Voxel(Voxel Voxel)
        {
            Yz = Voxel.Yz;
            Sz = Voxel.Sz;

            V = new int[3] { Voxel.V[0], Voxel.V[1], Voxel.V[2] };
            S = new int[3] { Voxel.S[0], Voxel.S[1], Voxel.S[2] };

            IsValid = true;
        }
        public Voxel(Voxel Voxel, int XZSize)
        {
            Yz = int.MaxValue;
            Sz = XZSize;

            V = new int[3] { Voxel.V[0], Voxel.V[1], Voxel.V[2] };
            S = new int[3] { Voxel.S[0], Voxel.S[1], Voxel.S[2] };
            IsValid = true;
        }


        //YXZ
        public int[] W { get; set;}
        

        public int[] V { get; internal set; }
        public int[] S { get; internal set; }

        public int Yz { get; internal set; }
        public int Sz { get; internal set; }

        public int RegionsPerWorld { get; set; } = 1;


        public int PixelsPerRegion
        {
            get
            {

                if ((512 % RegionsPerWorld) == 0)
                {
                    return 512 / RegionsPerWorld;
                }
                else
                {
                    return (512 / RegionsPerWorld) + 1;

                }
            }
        }

        public int WorldRegionPixelSize
        {
            get
            {
                if ((512 % RegionsPerWorld) == 0)
                {
                    return 512;
                }
                else
                {
                    return ((512 / RegionsPerWorld) + 1) * RegionsPerWorld;

                }
            }
        }

        public int BlocksPerWorld
        {
            get
            {
                if (RegionsPerWorld > 1)  return 512 * RegionsPerWorld; 
                else return 512;

            }
        }
        
        public void SetVoxel(Voxel Voxel)
        {
            SetVoxel(Voxel.WorldY, Voxel.WorldX, Voxel.WorldZ);
        }
        public void SetVoxel(Voxel Voxel,int XZSize)
        {
            Sz = XZSize;
            SetVoxel(Voxel.WorldY, Voxel.WorldX, Voxel.WorldZ);
        }

        public void SetVoxel(int WorldY, int WorldX, int WorldZ)
        {
            S[0] = Segment(Yz, WorldY);
            S[1] = Segment(Sz, WorldX);
            S[2] = Segment(Sz, WorldZ);

            V[0] = Offset(Yz, WorldY);
            V[1] = Offset(Sz, WorldX);
            V[2] = Offset(Sz, WorldZ);
        }
        public void SetVoxel(int WorldY, int WorldX, int WorldZ, int XZSize)
        {

            Sz = XZSize;

            SetVoxel(WorldY, WorldX, WorldZ);

        }
        public void SetVoxel(int WorldY, int WorldX, int WorldZ, int YSize, int XZSize)
        {
            Yz = YSize;
            Sz = XZSize;

            SetVoxel(WorldY, WorldX, WorldZ);

        }


        public void SetSegment(int xSeg, int zSeg)
        {
            S[1] = xSeg;
            S[2] = zSeg;

        }
        public void SetSegmentOffset(int xSeg, int zSeg, int xOff, int zOff)
        {
            S[1] = xSeg;
            S[2] = zSeg;
            V[1] = xOff;
            V[2] = zOff;


        }

        public Voxel SegmentAlignedVoxel()
        {
            return SegmentAlignedVoxel(Yz, Sz);
        }

        public Voxel SegmentAlignedVoxel(int RegionsPerWorld)
        {
            Voxel V = SegmentAlignedVoxel(Yz, RegionsPerWorld * 512);
            V.RegionsPerWorld = RegionsPerWorld;

            return V;

        }

        public Voxel SegmentAlignedVoxel(int YSize, int XZSize)
        {
            return new Voxel(YSize, XZSize) { WorldY = Ordinate(Yz, S[0], 0), WorldX = Ordinate(Sz, S[1], 0), WorldZ = Ordinate(Sz, S[2], 0) };
        }

        public Voxel OffsetVoxel(int YSize, int XZSize)
        {
            return new Voxel(YSize, XZSize) { WorldY = V[0], WorldX = V[1], WorldZ = V[2] };
        }

        public void SetOffset(int y, int x, int z)
        {
            V[0] = y;
            V[1] = x;
            V[2] = z;
        }
        public void SetOffset(Voxel Voxel)
        {
            V[0] = Voxel.WorldY;
            V[1] = Voxel.WorldX;
            V[2] = Voxel.WorldZ;

        }

        public int WorldY
        {
            get { return Ordinate(Yz, S[0], V[0]); }
            set { S[0] = Segment(Yz, value); V[0] = Offset(Yz, value); }
        }
        public int WorldX
        {
            get { return Ordinate(Sz, S[1], V[1]); }
            set { S[1] = Segment(Sz, value); V[1] = Offset(Sz, value); }
        }
        public int WorldZ
        {
            get { return Ordinate(Sz, S[2], V[2]); }
            set { S[2] = Segment(Sz, value); V[2] = Offset(Sz, value); }
        }

        public int Ys { get { return S[0]; } set { S[0] = value; } }
        public int Xs { get { return S[1]; } set { S[1] = value; } }
        public int Zs { get { return S[2]; } set { S[2] = value; } }

        public int Yo { get { return V[0]; } set { V[0] = value; } }
        public virtual int Xo { get { return V[1]; } set { V[1] = value; } }
        public virtual int Zo { get { return V[2]; } set { V[2] = value; } }


        public int ScaledXo { get => V[1] / RegionsPerWorld; set => V[1] = value * RegionsPerWorld; }
        public int ScaledZo { get => V[2] / RegionsPerWorld; set => V[2] = value * RegionsPerWorld; }
        
        public bool IsValid { get; set; }

        public int ChunkIdx()
        {
            return (S[2] * 32) + S[1];
        }
        public int ChunkZXIdx()
        {
            return (V[2] * 16) + V[1];
        }
        public int ChunkBlockPos()
        {
            return (V[0] * 16 * 16) + (V[2] * 16) + V[1];
        }

        public virtual bool Parse(string CsvXYZ)
        {
            float tf = 0;
            Voxel pV = this;

            StringBuilder sb = new StringBuilder();

            sb.Append(CsvXYZ);
            sb.Replace(' ', ',');


            string[] tpdata = sb.ToString().Split(',');

            if (tpdata.Length > 2)
            {

                if (float.TryParse(tpdata[0], out tf))
                {
                    pV.WorldX = (int)tf;
                    tf = 0;
                    if (float.TryParse(tpdata[1], out tf))
                    {
                        pV.WorldY = (int)tf;
                        tf = 0;
                        if (float.TryParse(tpdata[2], out tf))
                        {
                            pV.WorldZ = (int)tf;
                            pV.IsValid = true;

                        }

                    }
                }
            }
            else if (tpdata.Length > 1)
            {
                if (float.TryParse(tpdata[0], out tf))
                {
                    pV.WorldX = (int)tf;
                    tf = 0;
                    if (float.TryParse(tpdata[1], out tf))
                    {
                        pV.WorldZ = (int)tf;
                        pV.IsValid = true;
                    }
                }
            }

            return pV.IsValid;
        }
        public virtual bool ParseSegment(string CsvYXZ)
        {
            float tf = 0;

            StringBuilder sb = new StringBuilder();

            sb.Append(CsvYXZ);
            sb.Replace(' ', ',');


            string[] tpdata = sb.ToString().Split(',');

            if (tpdata.Length > 2)
            {

                if (float.TryParse(tpdata[0], out tf))
                {
                    WorldY = (int)tf;
                    tf = 0;
                    if (float.TryParse(tpdata[1], out tf))
                    {
                        Xs = (int)tf;
                        tf = 0;
                        if (float.TryParse(tpdata[2], out tf))
                        {
                            Zs = (int)tf;
                            IsValid = true;

                        }

                    }
                }
            }
            else if (tpdata.Length > 1)
            {
                if (float.TryParse(tpdata[0], out tf))
                {
                    Xs = (int)tf;
                    tf = 0;
                    if (float.TryParse(tpdata[1], out tf))
                    {
                        Zs = (int)tf;
                        IsValid = true;
                    }
                }
            }

            return IsValid;
        }
        public virtual bool ParseSegment(string CsvYXZ, params int[] Offset)
        {
            if (ParseSegment(CsvYXZ) == true)
            {

             
                if (Offset.Length > 2)
                {
                    SetOffset(Offset[0], Offset[1], Offset[2]);
                }
                else if (Offset.Length > 1)
                {
                    Xo = Offset[0];
                    Zo = Offset[1];
                }
            }

            return IsValid;

        }
    }

    
}