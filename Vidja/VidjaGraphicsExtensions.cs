using System.Drawing;

namespace Vidja
{
    public static class VidjaGraphicsExtensions
    {
        public static void FillCircle(this Graphics graphics, Brush brush, float x, float y, float radius)
        {
            var diameter = radius * 2;
            graphics.FillEllipse(brush, x - radius, y - radius, diameter, diameter);
        }
    }
}