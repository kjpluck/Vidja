using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vidja;

namespace Example02
{
    public class Program
    {
        public static void Main()
        {
            Vidja.Vidja.Generate(new Eg02(),"Eg02.gif");
        }
    }

    internal class Eg02 : IVidja
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

            for (var i = 0; i < 20; i++)
            {
                var angle = 2 * Math.PI * (1.0 * i / 20 + t / Duration);
                var centerX = (float)(Width * (0.5 + Math.Cos(angle) * 0.1));
                var centerY = (float)(Width * (0.5 + Math.Sin(angle) * 0.1));
                var brush = new SolidBrush(Color.FromArgb(255 * (i % 2), 255 * (i % 2), 255 * (i % 2)));
                var radius = (float)(Width * (1.0 - 1.0 * i / 20));
                graphics.FillCircle(brush, centerX, centerY, radius);
            }

            return bmp;
        }
    }
}
