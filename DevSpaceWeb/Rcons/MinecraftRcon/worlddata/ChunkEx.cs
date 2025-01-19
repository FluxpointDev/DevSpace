namespace LibMCRcon.WorldData
{

    public class ChunkEx
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;

        public bool EqualsXZ(VoxelEx obj)
        {
            return obj.X == X && obj.Z == Z;
        }

        public VoxelEx Copy()
        {
            return new VoxelEx() { X = X, Z = Z, Y = Y };
        }

        public void ToOrdinate(VoxelEx vxSegment, VoxelEx vxOffset)
        {
            X = Ordinate(1, vxSegment.X, vxOffset.X, 16);
            Z = Ordinate(1, vxSegment.Z, vxOffset.Z, 16);
            Y = Ordinate(1, vxSegment.Y, vxOffset.Y, 256);
        }
        public void ToSegment(VoxelEx vxOrdinate)
        {
            X = Segment(1, vxOrdinate.X, 16);
            Z = Segment(1, vxOrdinate.Z, 16);
            Y = Segment(1, vxOrdinate.Y, 256);
        }
        public void ToOffset(VoxelEx vxOrdinate)
        {
            X = Offset(1, vxOrdinate.X, 16);
            Z = Offset(1, vxOrdinate.Z, 16);
            Y = Offset(1, vxOrdinate.Y, 256);
        }
        public void ToWorldMap(VoxelEx vxOrdinate)
        {
            var vx = new VoxelEx();
            vx.ToOffset(vxOrdinate);

            X = Segment(1, vx.X, 16);
            Z = Segment(1, vx.Z, 16);
            Y = vxOrdinate.Y;

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

}