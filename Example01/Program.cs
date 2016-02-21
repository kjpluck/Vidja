
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vidja;

namespace Example01
{
    public class Program
    {
        public static void Main()
        {
            Vidja.Vidja.Generate(new Eg01(), "Eg01.gif");
        }
        
    }

    public class Eg01:IVidja
    {
        public int Width { get; } = 128;
        public int Height { get; } = 128;
        public int Fps { get; } = 15;
        public double Duration { get; } = 2;

        public Bitmap RenderFrame(double t)
        {
            var bmp = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var radius = (float)(Width * (1 + Math.Pow((t * (Duration - t)), 2)) / 6);
            graphics.FillCircle(Brushes.Red, 64, 64, radius);
            return bmp;
        }
    }
}
