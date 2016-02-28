using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vidja;

namespace Example05
{
    class Program
    {
        static void Main()
        {
            Vidja.Vidja.Generate(new Eg05(), "Eg05.gif");
        }
    }

    public class Eg05 : IVidja
    {
        public int Width { get; } = 256;
        public int Height { get; } = 256;
        public int Fps { get; } = 20;
        public double Duration { get; } = 2;


        private const int DisksPerCycle = 8;
        private const double Speed = .05;
        private const int NumberOfDisks = (int) (DisksPerCycle/Speed);
        private const double T0 = 1.0/Speed;                            // indicates at which avancement to start
        
        public Bitmap RenderFrame(double t)
        {
            var dt = 1.0*Duration/2/DisksPerCycle;
            var bmp = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (var i = 0; i < NumberOfDisks; i++)
            {

                var a = (Math.PI/DisksPerCycle)*(NumberOfDisks - i - 1);
                var r = Math.Max(0, .05*(t + T0 - dt*(NumberOfDisks - i - 1)));
                var centerX = (float) (Width*(0.5 + r*Math.Cos(a)));
                var centerY = (float) (Width*(0.5 + r*Math.Sin(a)));
                var grey = GetColour(i);
                var brush = new SolidBrush(Color.FromArgb(255, grey, grey, grey));
                graphics.FillCircle(brush, centerX, centerY, 0.3f*Width);
                graphics.DrawCircle(new Pen(Color.Black, 0.01f*Width), centerX, centerY, 0.3f*Width);
            }

            var halfW = Width/2f;
            graphics.DrawCircle(new Pen(Color.Black, halfW), halfW, halfW, 0.65f * Width);
            graphics.DrawCircle(new Pen(Color.White, 0.02f * Width), halfW, halfW, 0.42f * Width);
            return bmp;
        }

        private int GetColour(int i)
        {
            var foo = (1.0*i/DisksPerCycle)%1.0;
            return (int) (255*foo);
        }
    }
}
