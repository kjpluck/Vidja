using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Media;
using Vidja;
using Color = System.Drawing.Color;

namespace Example03
{
    public class Program
    {
        public static void Main()
        {
            Vidja.Vidja.Generate(new Eg03(), "Eg03.gif");
        }
    }

    public class Eg03 : IVidja
    {
        public int Width { get; } = 150;
        public int Height { get; } = 150;
        public int Fps { get; } = 15;
        public double Duration { get; } = 2;

        private readonly List<int> _radii = new List<int>();
        private readonly List<Color> _colors  = new List<Color>();
        private readonly List<Point> _centres = new List<Point>();
        private const int NumBalls = 60;

        public Eg03()
        {
            var rnd = new Random();

            for (var i = 0; i < NumBalls; i++)
            {
                _radii.Add(rnd.Next((int) (0.1*Width), (int) (0.2*Width)));
                _colors.Add(Color.FromArgb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                _centres.Add(new Point(rnd.Next(Width), rnd.Next(Width)));
            }

            
        }

        public Bitmap RenderFrame(double t)
        {
            var bmp = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (var i = 0; i < NumBalls; i++)
            {
                var angle = 2*Math.PI*(t/Duration*Math.Sign(_colors[i].B-128) + ((float)_colors[i].G/255));
                var x = (float) (_centres[i].X + Width / 5 * Math.Cos(angle));
                var y = (float) (_centres[i].Y + Width / 5 * Math.Sin(angle));
                var colour = new SolidBrush(_colors[i]);
                graphics.FillCircle(colour, x, y, _radii[i]);
            }

            return bmp;
        }
    }
}
