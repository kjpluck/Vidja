using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace Vidja
{
    public static class Vidja
    {
        public static void Generate(IVidja vidja, string filename = "vidja.mp4", int startAt = 0)
        {
            var ffMpegPath = ConfigurationManager.AppSettings["FFmpegExe"];

            if (!File.Exists(ffMpegPath))
            {
                throw new InvalidOperationException("Vidja uses FFmpeg to generate video output.  If you have it installed please set the path to ffmpeg.exe in App.config otherwise please download FFmpeg from http://ffmpeg.org/");
            }

            var ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ConfigurationManager.AppSettings["FFmpegExe"],
                    Arguments = $"-y -video_size {vidja.Width}x{vidja.Height} -r {vidja.Fps} -i pipe:0 -pix_fmt bgr24 {filename}",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            ffmpeg.Start();

            var input = ffmpeg.StandardInput.BaseStream;

            using (var writer = new BinaryWriter(input))
            {
                var frameCount = (int)Math.Round(vidja.Fps * vidja.Duration);

                var startFrame = startAt * vidja.Fps;

                for (var frameCounter = startFrame; frameCounter < frameCount; frameCounter++)
                {
                    var t = frameCounter / (double)vidja.Fps;
                    var bitmap = vidja.RenderFrame(t);

                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Bmp);
                        var buffer = ms.ToArray();

                        writer.Write(buffer, 0, buffer.Length);
                    }
                }
                writer.Close();
                ffmpeg.WaitForExit();
            }
        }
    }
}
