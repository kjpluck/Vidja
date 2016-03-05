using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace Vidja
{
    public static class Vidja
    {
        private static Process _ffmpeg;
        private static IVidja _vidja;
        private static string _ffMpegPath;
        private static int _startAt;
        private static string _outputFilename;
        private static readonly string TempDir = Environment.GetEnvironmentVariable("TEMP");

        public static void Generate(IVidja vidja, string outputFilename = "vidja.mp4", int startAt = 0)
        {
            _outputFilename = outputFilename;
            _startAt = startAt;
            _vidja = vidja;

            CheckForFfMpegExe();

            if (OutputIsAGif(outputFilename))
            {
                // The default gif palette used by FFMpeg makes mediocre gifs
                // but it can accept a custom palette which FFMpeg can also generate (which is nice)
                // First we run all the frames through FFMpeg to generate this palette which is simply a png saved in the temp directory
                ExtractPalette();

                // Then create the gif using that palette.
                // TODO cache the generated frames from above
                CreateGifAnimation();

                DeletePalette();
            }
            else
            {
                CreateAnimation();
            }
        }

        private static bool OutputIsAGif(string outputFilename)
        {
            return outputFilename.ToLower().EndsWith(".gif");
        }

        private static void ExtractPalette()
        {
            StartFfMpegProcess($"-i pipe:0 -vf \"palettegen\" -y {TempDir}\\palette.png");
            PipeFramesToFfMpegProcess();
        }

        private static void CreateGifAnimation()
        {
            StartFfMpegProcess($"-i pipe:0 -i {TempDir}\\palette.png -lavfi \"fps={_vidja.Fps} [x]; [x][1:v] paletteuse\" -y {_outputFilename}");
            PipeFramesToFfMpegProcess();
        }

        private static void CreateAnimation()
        {
            StartFfMpegProcess($"-y -video_size {_vidja.Width}x{_vidja.Height} -r {_vidja.Fps} -i pipe:0 -pix_fmt bgr24 {_outputFilename}");
            PipeFramesToFfMpegProcess();
        }

        private static void DeletePalette()
        {
            File.Delete($"{TempDir}\\palette.png");
        }

        private static void StartFfMpegProcess(string arguments)
        {
            _ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffMpegPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            _ffmpeg.Start();
        }

        private static void CheckForFfMpegExe()
        {
            _ffMpegPath = ConfigurationManager.AppSettings["FFmpegExe"];

            if (!File.Exists(_ffMpegPath))
            {
                throw new InvalidOperationException(
                    "Don't be dismayed! Just one more step: Vidja uses FFmpeg to generate the animation output.  If you have it installed please set the path to ffmpeg.exe in App.config otherwise please download FFmpeg from http://ffmpeg.org/");
            }
        }

        private static void PipeFramesToFfMpegProcess()
        {
            var input = _ffmpeg.StandardInput.BaseStream;

            using (var writer = new BinaryWriter(input))
            {
                var frameCount = (int) Math.Round(_vidja.Fps*_vidja.Duration);

                var startFrame = _startAt*_vidja.Fps;

                for (var frameCounter = startFrame; frameCounter < frameCount; frameCounter++)
                {
                    var t = frameCounter/(double) _vidja.Fps;
                    WriteFrameToFfMpegProcess(t, writer);
                }
                writer.Close();
                _ffmpeg.WaitForExit();
            }
        }

        private static void WriteFrameToFfMpegProcess(double t, BinaryWriter writer)
        {
            var bitmap = _vidja.RenderFrame(t);

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                var buffer = ms.ToArray();

                writer.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
