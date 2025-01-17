
namespace gg.Mesh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Vertex
    {
        public Vertex()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public int Index { get; set; }

        public float DeltaSquaredXY(Vertex t)
        {
            float dx = (X - t.X);
            float dy = (Y - t.Y);
            return (dx * dx) + (dy * dy);
        }

        public float DeltaSquared(Vertex t)
        {
            float dx = (X - t.X);
            float dy = (Y - t.Y);
            float dz = (Z - t.Z);
            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        public float DistanceXY(Vertex t)
        {
            return (float)System.Math.Sqrt(DeltaSquaredXY(t));
        }

        public float Distance(Vertex t)
        {
            return (float)System.Math.Sqrt(DeltaSquared(t));
        }

        public bool InsideXY(System.Drawing.RectangleF region)
        {
            if (X < region.Left) return false;
            if (X > region.Right) return false;
            if (Y < region.Top) return false;
            if (Y > region.Bottom) return false;
            return true;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
