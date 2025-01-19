namespace LibMCRcon.WorldData
{

    public abstract class VoxelBase
    {

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public int R { get; set; } = 0;

        public bool EqualsXZR<T>(T obj) where T: VoxelBase
        {
            return obj.X == X && obj.Z == Z && obj.R == R;
        }
        public bool EqualsXZ<T>(T obj) where T : VoxelBase
        {
            return obj.X == X && obj.Z == Z;
        }
        public static int Segment(int Regions, int Ordinate, int Size = 512)
        {
            return Ordinate < 0 ? (((Ordinate + 1) / (Size * Regions)) - 1) : (Ordinate / (Size * Regions));
        }
        public static int Offset(int Regions, int Ordinate, int Size = 512)
        {
            return (Ordinate - (Segment(Regions, Ordinate, Size) * (Size * Regions))) / Regions;
        }
        public static int Ordinate(int Regions, int Segment, int Offset, int Size = 512)
        {
            if (Segment < 0)
                return -((-Segment * (Regions * Size)) - (Offset * Regions));
            else
                return (Segment * (Regions * Size)) + (Offset * Regions);

        }
        
    }

    public class ChunkVoxelEx:VoxelBase
    {
        private new readonly int R = 1;

        public int Xs { get => Segment(R, X, 16); set => X = Ordinate(R, value, Offset(R, X, 16)); }
        public int Zs { get => Segment(R, Z, 16); set => Z = Ordinate(R, value, Offset(R, X, 16)); }

        public int Xo { get => Offset(R, X, 16); set => X = Ordinate(R, Segment(R, X, 16), value); }
        public int Zo { get => Offset(R, Z, 16); set => Z = Ordinate(R, Segment(R, Z, 16), value); }

        public int XsToRegion(int Regions) => Segment(Regions, X, 16);
        public int ZsToRegion(int Regions) => Segment(Regions, Z, 16);

        public int XoToRegion(int Regions) => Offset(Regions, X, 16);
        public int ZoToRegion(int Regions) => Offset(Regions, Z, 16);

        public bool EqualsXZR(WorldVoxelEx obj) => base.EqualsXZR(obj);

        public OffsetVoxelEx CopyToOffset(int Regions) => new OffsetVoxelEx() { R = Regions, WorldX = X, WorldZ = Z, Y = Y };
        public SegmentVoxelEx CopyToSegment(int Regions) => new SegmentVoxelEx() { R = Regions, WorldX = X, WorldZ = Z, Y = Y };
        public WorldVoxelEx CopyToWorld(int Regions) => new WorldVoxelEx() { R = Regions, X = X, Z = Z, Y = Y };
    }
    public class WorldVoxelEx:VoxelBase
    {
        public WorldVoxelEx() { R = 1; X = 0; Y = 0; Z = 0; }
        public WorldVoxelEx(int Regions){ R = Regions; }
        public WorldVoxelEx(int Xs, int Zs, int Xo, int Zo, int Y, int R)
        {
            Y = Offset(1, Y, 256);
            X = Ordinate(R, Xs, Xo);
            Z = Ordinate(R, Zs, Zo);
            
            this.R = R;
        }
        public WorldVoxelEx(int Xs, int Zs, int R)
        {
            X = Ordinate(R, Xs, 0);
            Z = Ordinate(R, Zs, 0);
            this.R = R;
        }
               

        public int Xs { get => Segment(R, X); set => X = Ordinate(R, value, Offset(R, X)); }
        public int Zs { get => Segment(R, Z); set => Z = Ordinate(R, value, Offset(R, X)); }

        public int Xo { get => Offset(R, X); set => X = Ordinate(R, Segment(R, X), value); }
        public int Zo { get => Offset(R, Z); set => Z = Ordinate(R, Segment(R, Z), value); }

        public int XsToRegion(int Regions) => Segment(Regions, X);
        public int ZsToRegion(int Regions) => Segment(Regions, Z);

        public int XoToRegion(int Regions) => Offset(Regions, X);
        public int ZoToRegion(int Regions) => Offset(Regions, Z);

        public bool EqualsXZR(WorldVoxelEx obj) => base.EqualsXZR(obj);

        public void Reset(WorldVoxelEx W) { R = W.R; X = W.X; Z = W.Z; Y = W.Y; }
        public void Reset(int Xs, int Zs, int Xo, int Zo, int Y, int R)
        {
            X = Ordinate(R, Xs, Xo);
            Z = Ordinate(R, Zs, Zo);

            this.Y = Offset(1, Y, 256);
            this.R = R;

        }

        public string BaseFilename()
        {

                return R > 1 ? $@".{Xs}.{Zs}.{R}" : $@".{Xs}.{Zs}";

        }


        public WorldVoxelEx CopyToWorld(int Regions) => new WorldVoxelEx() { R = Regions, X = X, Z = Z, Y = Y };


        public static int PixelSize(int DestRegions, int SrcRegions = 1)
        {
            if (DestRegions < SrcRegions)
                return 0;

            if (DestRegions < 1 || SrcRegions < 1)
                return 0;

            return (512 / DestRegions) * SrcRegions;
        }




    }
    public class SegmentVoxelEx : VoxelBase
    {

        public SegmentVoxelEx() { }
        public SegmentVoxelEx(int Xs, int Zs, int Regions) { R = Regions; X =Xs; Z = Zs; }
        

        
        public int Xo { get; set; } = 0;
        public int Zo { get; set; } = 0;

        public int WorldX
        {
            get => Ordinate(R, X, Xo);
            set
            {
                Xo = Offset(R, value);
                X = Segment(R, value);
            }
        }
        public int WorldZ
        {
            get => Ordinate(R, Z, Zo);
            set
            {
                Zo = Offset(R, value);
                Z = Segment(R, value);
            }
        }
        
        public bool EqualsXZR(SegmentVoxelEx obj) => base.EqualsXZR(obj);
        
        public void ToRegions(int Regions)
        {
            var Xw = Ordinate(R, X, Xo);
            var Zw = Ordinate(R, Z, Zo);

            R = Regions;

            WorldX = Xw;
            WorldZ = Zw;

        }

        public WorldVoxelEx CopyToWorld(int Regions) => new WorldVoxelEx() { R = Regions, X = Ordinate(R, X, Xo), Z = Ordinate(R, Z, Zo), Y = Y };
        public SegmentVoxelEx CopyToSegment(int Regions) => new SegmentVoxelEx() { R = Regions, WorldX = WorldX, WorldZ = WorldZ, Y = Y };

    }
    public class OffsetVoxelEx:VoxelBase
    {
        
        public int Xs { get; set; } = 0;
        public int Zs { get; set; } = 0;

        public int WorldX
        {
            get => Ordinate(R, Xs, X);
            set
            {
                X = Offset(R, value);
                Xs = Segment(R, value);
            }
        }
        public int WorldZ
        {
            get => Ordinate(R, Zs, Z);
            set
            {
                Z = Offset(R, value);
                Zs = Segment(R, value);
            }
        }

        public bool EqualsXZ(OffsetVoxelEx obj) => base.EqualsXZ(obj);

        public void ToRegions(int Regions)
        {
            var Xw = Ordinate(R, Xs, X);
            var Zw = Ordinate(R, Zs, Z);

            R = Regions;

            WorldX = Xw;
            WorldZ = Zw;

        }

        public WorldVoxelEx CopyToWorld(int Regions) => new WorldVoxelEx() { R = Regions, X = Ordinate(R, Xs, X), Z = Ordinate(R, Zs, Z), Y = Y };
    }
    public class VoxelEx:VoxelBase
    {
        public VoxelEx Copy() => new VoxelEx() { X = X, Z = Z, Y = Y, R = R };

        public void ToOrdinate(VoxelEx vxSegment, VoxelEx vxOffset, int Regions = 1)
        {
            X = Ordinate(Regions, vxSegment.X, vxOffset.X);
            Z = Ordinate(Regions, vxSegment.Z, vxOffset.Z);
            Y = Ordinate(1, vxSegment.Y, vxOffset.Y, 256);
        }
        public void ToSegment(VoxelEx vxOrdinate, int Regions = 1)
        {
            X = Segment(Regions, vxOrdinate.X);
            Z = Segment(Regions, vxOrdinate.Z);
            Y = Segment(Regions, vxOrdinate.Y, 256);
        }
        public void ToOffset(VoxelEx vxOrdinate, int Regions = 1)
        {
            X = Offset(Regions, vxOrdinate.X);
            Z = Offset(Regions, vxOrdinate.Z);
            Y = Offset(1, vxOrdinate.Y, 256);
        }
        public void ToWorldMap(VoxelEx vxOrdinate, int Regions)
        {
            var vx = new VoxelEx();
            vx.ToOffset(vxOrdinate, Regions);

            X = Segment(1, vx.X, 512 / Regions);
            Z = Segment(1, vx.Z, 512 / Regions);
            Y = vxOrdinate.Y;

        }

 

    }


}