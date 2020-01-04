using System;
using System.Windows.Media;
using WSharp.Wpf.Colors;

namespace WSharp.Wpf.Extensions
{
    public static class HslExtensions
    {
        internal static Color ToColor(this Hsl hsl)
        {
            static double HsvRbg(double v1, double v2, double vH)
            {
                if (vH < 0) vH += 1;
                if (vH > 1) vH -= 1;
                return (6 * vH) < 1 
                    ? v1 + ((v2 - v1) * 6 * vH) 
                    : (2 * vH) < 1 
                    ? v2 
                    : (3 * vH) < 2 
                    ? v1 + ((v2 - v1) * ((2.0 / 3) - vH) * 6) 
                    : v1;
            }

            var h = hsl.H * (1.0 / 360);
            var s = hsl.S * (1.0 / 100);
            var l = hsl.L * (1.0 / 100);

            double r, g, b;
            if (s == 0)
            {
                r = l * 255;
                g = l * 255;
                b = l * 255;
            }
            else
            {
                var var_2 = l < 0.5 
                    ? l * (1 + s) 
                    : l + s - (s * l);

                var var_1 = (2 * l) - var_2;

                r = 255 * HsvRbg(var_1, var_2, h + (1.0 / 3));
                g = 255 * HsvRbg(var_1, var_2, h);
                b = 255 * HsvRbg(var_1, var_2, h - (1.0 / 3));
            }

            return Color.FromRgb((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }
    }
}
