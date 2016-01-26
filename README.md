# Vidja
.net video creator library

Example: https://youtu.be/Q_a-STbdDYU

First download FFmpeg from http://ffmpeg.org.

Create a command line project.

    Install-Package vidja

Add the FFmpeg executable path to app.config

Create a class that implements the IVidja interface.

Set the `Width`, `Height`, `FPS` (Frames per second), `Duration` (in seconds) fields.

Implement `RenderFrame(double t)` so that given a time from 0 to `t` return a bitmap of the above width and height.

    using System.Drawing;
    using Vidja;

    namespace VidjaHelloWorld
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                Vidja.Vidja.Generate(new HelloWorld());
            }
        }

        public class HelloWorld:IVidja
        {

            public int Width { get; } = 1000;
            public int Height { get; } = 1000;
            public int Fps { get; } = 25;
            public double Duration { get; } = 10;

            public Bitmap RenderFrame(double t)
            {

                var bmp = new Bitmap(Width, Height);
                var graphics = Graphics.FromImage(bmp);

                graphics.DrawString($"Hello, World! {(int)t}", new Font("Arial", 32), new SolidBrush(Color.GreenYellow),
                    new Rectangle(0, 0, Width, Height), _centeredText);

                return bmp;
            }

            private readonly StringFormat _centeredText = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
        }
    }

Run `VidjaHelloWorld.exe` which will generate `Vidja.mp4` and view it in your favourite video player.

