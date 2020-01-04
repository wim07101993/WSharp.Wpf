using System;
using System.Windows.Media;
using WSharp.Wpf.Colors;

namespace WSharp.Wpf.Extensions
{
    internal static class XyzExtensions
    {
        public static Color ToColor(this Xyz xyz)
        {
            static double XyzRgb(double d)
            {
                return d > 0.0031308 
                    ? 255.0 * ((1.055 * Math.Pow(d, 1.0 / 2.4)) - 0.055) 
                    : 255.0 * (12.92 * d);
            }

            static byte Clip(double d)
            {
                if (d < 0) return 0;
                if (d > 255) return 255;
                return (byte)Math.Round(d);
            }

            var r = XyzRgb((3.2404542 * xyz.X) - (1.5371385 * xyz.Y) - (0.4985314 * xyz.Z));
            var g = XyzRgb((-0.9692660 * xyz.X) + (1.8760108 * xyz.Y) + (0.0415560 * xyz.Z));
            var b = XyzRgb((0.0556434 * xyz.X) - (0.2040259 * xyz.Y) + (1.0572252 * xyz.Z));

            return Color.FromRgb(Clip(r), Clip(g), Clip(b));
        }

        public static Lab ToLab(this Xyz xyz)
        {
            static double XyzLab(double v)
            {
                return v > LabConstants.E 
                    ? Math.Pow(v, 1 / 3.0) 
                    : ((v * LabConstants.K) + 16) / 116;
            }

            var fx = XyzLab(xyz.X / LabConstants.WhitePointX);
            var fy = XyzLab(xyz.Y / LabConstants.WhitePointY);
            var fz = XyzLab(xyz.Z / LabConstants.WhitePointZ);

            var l = (116 * fy) - 16;
            var a = 500 * (fx - fy);
            var b = 200 * (fy - fz);

            return new Lab(l, a, b);
        }
    }
}
