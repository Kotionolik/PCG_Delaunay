

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gg.Mesh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        List<Vertex> Set = new List<Vertex>();

        private void button1_Click(object sender, EventArgs e)
        {
            Set = MakeTestSet(640, 480, (int)numericUpDown2.Value, (int)numericUpDown1.Value);

            Bitmap b = pictureBox1.Image != null ? (Bitmap)pictureBox1.Image : new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            drawSet(b);
            pictureBox1.Image = b;
        }

        protected void drawSet(Bitmap b)
        {
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.White);
                
                for (int i = 0; i < Set.Count; i++)
                {
                    g.DrawEllipse(Pens.Black, Set[i].X - 1, Set[i].Y - 1, 2, 2);    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap b = pictureBox1.Image != null ? (Bitmap)pictureBox1.Image : new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Set = MakeTestSet(640, 480, (int)numericUpDown2.Value, (int)numericUpDown1.Value);
            Mesh m = new Mesh();
            m.Recursion = (int)numericUpDown3.Value;
            System.DateTime start = System.DateTime.Now;
            m.Compute(Set, new RectangleF(0, 0, 640, 480));
            label3.Text = System.DateTime.Now.Subtract(start).TotalMilliseconds.ToString() + " msec";
            drawSet(b);
            using (Graphics g = Graphics.FromImage(b))
            {
                m.Draw(g, 0, 0, 640, 680);
            }
            pictureBox1.Image = b;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            runCount = 0;
            timer1.Enabled = true;
        }

        int runCount = 0;

        public List<Vertex> MakeTestSet(int width, int height, int seed, int count)
        {
            List<Vertex> set = new List<Vertex>();
            Random.Random r = new Random.Random(seed);
            for (int i = 0; i < count; i++)
            {
                set.Add(new Vertex(r.NextFlat_Int(0, width), r.NextFlat_Int(0, height), 0));
            }
            return set;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (runCount > 10)
            {
                timer1.Enabled = false;
                runCount = 0;
            }

            runCount++;
            Set = MakeTestSet(640, 480, runCount + (int)numericUpDown2.Value, (int)numericUpDown1.Value);
            Mesh m = new Mesh();
            m.Recursion = (int)numericUpDown3.Value;
            Bitmap b = pictureBox1.Image != null ? (Bitmap)pictureBox1.Image : new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.DateTime start = System.DateTime.Now;
            m.Compute(Set, new RectangleF(0, 0, 640, 480));
            label3.Text = System.DateTime.Now.Subtract(start).TotalMilliseconds.ToString() + " msec";
            drawSet(b);
            using (Graphics g = Graphics.FromImage(b))
            {
                m.Draw(g, 0, 0, 640, 480);
            }
            pictureBox1.Image = b;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            button2_Click(sender, e);
        }
    }
}
