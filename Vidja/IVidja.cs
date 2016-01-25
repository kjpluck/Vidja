using System.Drawing;

namespace Vidja
{
    public interface IVidja
    {
        int Width { get; }
        int Height { get; }
        int Fps { get; }
        double Duration { get; }

        Bitmap RenderFrame(double t);
    }
}