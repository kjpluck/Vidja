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

![Alt text](/Examples/Output/Eg01.gif?raw=true "Optional Title")

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

