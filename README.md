# Vidja
.net video creator library

Inspired/ported/stolen/plagiarised from http://zulko.github.io/blog/2014/09/20/vector-animations-with-python/

Example: https://youtu.be/Q_a-STbdDYU

First download FFmpeg from http://ffmpeg.org.

Create a command line project.

    Install-Package vidja

Add the FFmpeg executable path to app.config

Create a class that implements the IVidja interface.

Set the `Width`, `Height`, `FPS` (Frames per second), `Duration` (in seconds) fields.

Implement `RenderFrame(double t)` so that given a time from 0 to `t` return a bitmap of the above width and height.

##Example 1

![Pulsing red ball](/Examples/Output/Eg01.gif?raw=true)

We start with an easy one. In `RenderFrame` we just draw a red circle, whose radius depends on the time `t`:

```c#
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
```

## Example 2

![Spiralling circles](/Examples/Output/Eg02.gif?raw=true)

Now there are more circles, and we start to see the interest of making animations programmatically using for loops.

```c#
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

```

##Example 3

![Dancing circles](/Examples/Output/Eg03.gif?raw=true)

I tried to replicate zulko's example here but was not able to get my head around WPF where the .net radial fill resides to make the circles look like balls.  If anyone out there is able to provide me an example I'm more than will to accept a pull request!

```c#
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

```

## Example 5

This is a derivative of a Dave Whyte animation. It is made of stacked circles moving towards the picture’s border, with carefully chosen sizes, starting times, and colors. The black around the picture is simply a big circle with no fill and a very very thick black border.

```c#
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

```
