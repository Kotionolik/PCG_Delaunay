

namespace gg.Mesh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Mesh
    {
        #region Protected data
        /// <summary>
        /// Recursion limit.
        /// </summary>
        protected int m_recursion = 4;

        /// <summary>
        /// The points.
        /// </summary>
        protected List<Vertex> m_points = new List<Vertex>();

        /// <summary>
        /// The facets.
        /// </summary>
        protected List<Triangle> m_facets = new List<Triangle>();

        /// <summary>
        /// Bounds
        /// </summary>
        protected System.Drawing.RectangleF m_bounds = new System.Drawing.RectangleF(0, 0, 640, 480);

        #endregion

        #region Properties: Points, Facets, Bounds, Recursion.
        /// <summary>
        /// The points.
        /// </summary>
        public List<Vertex> Points
        {
            get { return m_points; }
            set { m_points = value; }
        }

        /// <summary>
        /// The facets.
        /// </summary>
        public List<Triangle> Facets
        {
            get { return m_facets; }
            set { m_facets = value; }
        }

        /// <summary>
        /// Bounds.
        /// </summary>
        public System.Drawing.RectangleF Bounds
        {
            get { return m_bounds; }
            set { m_bounds = value; }
        }

        /// <summary>
        /// Recursion level.
        /// </summary>
        public int Recursion 
        { 
            get { return m_recursion; } 
            set { if (value < 0) value = 0; m_recursion = value; } 
        }

        #endregion

        public int[] GetVertexIndicies()
        {
            int[] indicies = new int[3 * Facets.Count];
            int k = 0;
            for (int i = 0; i < m_points.Count; i++)
            {
                m_points[i].Index = i;
            }
            for (int i = 0; i < Facets.Count; i++)
            {
                indicies[k++] = Facets[i].A.Index;
                indicies[k++] = Facets[i].B.Index;
                indicies[k++] = Facets[i].C.Index;
            }
            return indicies;
        }

        public void Compute(List<Vertex> set, System.Drawing.RectangleF bounds)
        {
            Setup(bounds);
            for (int i = 0; i < set.Count; i++)
            {
                Append(set[i]);
            }
        }

        public void Append(Vertex v)
        {
            for (int i = 0; i < Facets.Count; i++)
            {
                if (Facets[i].Contains(v))
                {
                    Insert(v, Facets[i]);
                }
            }
        }

        public void Setup(System.Drawing.RectangleF bounds)
        {
            Triangle.ResetIndex();
            Facets.Clear();
            Points.Clear();
            Bounds = bounds;

            Vertex tl = new Vertex(Bounds.Left, Bounds.Top, 0);
            Vertex tr = new Vertex(Bounds.Right, Bounds.Top, 0);
            Vertex bl = new Vertex(Bounds.Left, Bounds.Bottom, 0);
            Vertex br = new Vertex(Bounds.Right, Bounds.Bottom, 0);
            Triangle t1 = new Triangle();
            Triangle t2 = new Triangle();
            t1.A = bl;
            t1.B = tr;
            t1.C = tl;
            t2.A = bl;
            t2.B = br;
            t2.C = tr;
            t1.AB = t2;
            t2.CA = t1;
            Facets.Add(t1);
            Facets.Add(t2);
        }


        public void Draw(System.Drawing.Graphics g, int minx, int miny, int maxx, int maxy)
        {
            System.Drawing.Pen[] pens = { 
                System.Drawing.Pens.Red, 
                System.Drawing.Pens.Green,
                System.Drawing.Pens.Blue, 
                System.Drawing.Pens.Orange,
                System.Drawing.Pens.Purple, 
                System.Drawing.Pens.Brown,
                System.Drawing.Pens.Violet, 
                System.Drawing.Pens.Lime,
                System.Drawing.Pens.DarkBlue, 
                System.Drawing.Pens.Magenta,
                System.Drawing.Pens.Cyan, 
                System.Drawing.Pens.DarkRed};

            maxx -= 2;
            maxy -= 2;
            for (int i = 0; i < Facets.Count; i++)
            {
                float x = Facets[i].OpositeOfEdge(0).X;
                float y = Facets[i].OpositeOfEdge(0).Y;
                int k = i % pens.Length;
                for (int j = 1; j < 4; j++)
                {
                    x = x < minx ? minx : x;
                    y = y < miny ? miny : y;
                    x = x > maxx ? maxx : x;
                    y = y > maxy ? maxy : y;

                    float nx = Facets[i].OpositeOfEdge(j).X;
                    float ny = Facets[i].OpositeOfEdge(j).Y;
                    nx = nx < minx ? minx : nx;
                    ny = ny < miny ? miny : ny;
                    nx = nx > maxx ? maxx : nx;
                    ny = ny > maxy ? maxy : ny;
                    g.DrawLine(pens[k], x, y, nx, ny);
                    x = nx;
                    y = ny;
                }
            }

        }
        protected void Insert(Vertex v, Triangle old)
        {
            if ((old.A.X == v.X) && (old.A.Y == v.Y)) return;
            if ((old.B.X == v.X) && (old.B.Y == v.Y)) return;
            if ((old.C.X == v.X) && (old.C.Y == v.Y)) return;

            m_points.Add(v);

            Triangle ab = new Triangle(old); 
            Triangle bc = new Triangle(old); 
            Triangle ca = new Triangle(old); 
            ab.C = v;
            bc.A = v;
            ca.B = v;

            ab.BC = bc;
            ab.CA = ca;
            bc.AB = ab;
            bc.CA = ca;
            ca.AB = ab;
            ca.BC = bc;

            Triangle[] ta = { ab.AB, bc.BC, ca.CA };
            Triangle[] tb = { ab, bc, ca };
            for (int j = 0; j < 3; j++)
            {
                if (ta[j] == null) continue;
                if (ta[j].Edge(0) == old)
                {
                    ta[j].SetEdge(0, tb[j]);
                    continue;
                }
                if (ta[j].Edge(1) == old)
                {
                    ta[j].SetEdge(1, tb[j]);
                    continue;
                }
                ta[j].SetEdge(2, tb[j]);
            }

            Facets.Add(ab);
            Facets.Add(bc);
            Facets.Add(ca);
            Facets.Remove(old);


            flipIfNeeded(ab, ab.AB, Recursion);
            flipIfNeeded(bc, bc.BC, Recursion);
            flipIfNeeded(ca, ca.CA, Recursion);

            return;
        }


        protected void flipIfNeeded(Triangle a, Triangle b, int depth)
        {
            if (depth <= 0) return;
            if (a == null) return;
            if (b == null) return;
            depth--;

            int ai = 0;
            int bi = 0;
            if (a.Edge(1) == b) ai = 1;
            if (a.Edge(2) == b) ai = 2;
            if (b.Edge(1) == a) bi = 1;
            if (b.Edge(2) == a) bi = 2;

            int[] table = { 2, 0, 1 };
            int vai = table[ai];
            int vbi = table[bi];

            float fa = a.VertexAngleRadians(vai);
            float fb = b.VertexAngleRadians(vbi);
            if (fa + fb <= System.Math.PI)
            {
                return;
            }


            Triangle[] ts = { a.Edge(0), a.Edge(1), a.Edge(2), b.Edge(0), b.Edge(1), b.Edge(2) };

            Vertex aOp = a.OpositeOfEdge(ai);
            Vertex bOp = b.OpositeOfEdge(bi);

            a.SetVertex(ai + 1, bOp);
            b.SetVertex(bi + 1, aOp);

            a.AB = null;
            a.BC = null;
            a.CA = null;
            b.AB = null;
            b.BC = null;
            b.CA = null;

            for (int i = 0; i < 6; i++)
            {
                if (ts[i] == null) continue;
                ts[i].RepairEdges(a);
                ts[i].RepairEdges(b);
            }

            flipIfNeeded(a, a.Edge(ai + 1), depth);
            flipIfNeeded(b, b.Edge(bi + 1), depth);
            flipIfNeeded(a, a.Edge(ai + 2), depth);
            flipIfNeeded(b, b.Edge(bi + 2), depth);
        }        
    }
}
