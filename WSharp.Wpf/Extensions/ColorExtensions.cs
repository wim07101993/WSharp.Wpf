using System;
using System.Linq;
using System.Windows.Media;

using WSharp.Wpf.Colors;

namespace WSharp.Wpf.Extensions
{
    public static class ColorExtensions
    {
        public static Color ContrastingForegroundColor(this Color color)
        {
            static double RgbSrgb(double d)
            {
                d /= 255.0;
                return (d > 0.03928)
                    ? _ = Math.Pow((d + 0.055) / 1.055, 2.4)
                    : _ = d / 12.92;
            }
            var r = RgbSrgb(color.R);
            var g = RgbSrgb(color.G);
            var b = RgbSrgb(color.B);

            var luminance = (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
            return luminance > 0.179
                ? System.Windows.Media.Colors.Black
                : System.Windows.Media.Colors.White;
        }

        public static Color ShiftLightness(this Color color, int amount = 1)
        {
            var lab = color.ToLab();
            var shifted = new Lab(lab.L - (LabConstants.Kn * amount), lab.A, lab.B);
            return shifted.ToColor();
        }

        public static Color Darken(this Color color, int amount = 1) => color.ShiftLightness(amount);

        public static Color Lighten(this Color color, int amount = 1) => color.ShiftLightness(-amount);

        /// <summary>Calculates the CIE76 distance between two colors.</summary>
        /// <param name="color"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double Difference(this Color color, Color other)
        {
            var lab1 = color.ToLab();
            var lab2 = other.ToLab();

            return Math.Sqrt(Math.Pow(lab2.L - lab1.L, 2) +
                             Math.Pow(lab2.A - lab1.A, 2) +
                             Math.Pow(lab2.B - lab1.B, 2));
        }

        internal static Lab ToLab(this Color c) => c.ToXyz().ToLab();

        internal static Xyz ToXyz(this Color c)
        {
            static double RgbXyz(double v)
            {
                v /= 255;
                return v > 0.04045
                    ? Math.Pow((v + 0.055) / 1.055, 2.4)
                    : v / 12.92;
            }

            var r = RgbXyz(c.R);
            var g = RgbXyz(c.G);
            var b = RgbXyz(c.B);

            var x = (0.4124564 * r) + (0.3575761 * g) + (0.1804375 * b);
            var y = (0.2126729 * r) + (0.7151522 * g) + (0.0721750 * b);
            var z = (0.0193339 * r) + (0.1191920 * g) + (0.9503041 * b);
            return new Xyz(x, y, z);
        }

        public static Hsb ToHsb(this Color color)
        {
            double r = color.R;
            double g = color.G;
            double b = color.B;

            r /= 255;
            g /= 255;
            b /= 255;

            var rgb = new[] { r, g, b };
            var max = rgb.Max();
            var min = rgb.Min();
            var v = max;
            var h = max;

            var d = max - min;
            var s = max.IsCloseTo(0) ? 0 : d / max;

            if (max.IsCloseTo(min))
                h = 0; // achromatic
            else
            {
                if (max.IsCloseTo(r))
                    h = ((g - b) / d) + (g < b ? 6 : 0);
                else if (max.IsCloseTo(g))
                    h = ((b - r) / d) + 2;
                else if (max.IsCloseTo(b))
                    h = ((r - g) / d) + 4;

                h *= 60;
            }

            return new Hsb(h, s, v);
        }

        public static Color ToColor(this Hsb hsv)
        {
            var h = hsv.Hue;
            var s = hsv.Saturation;
            var b = hsv.Brightness;

            b *= 255;

            if (s.IsCloseTo(0))
                return Color.FromRgb((byte)b, (byte)b, (byte)b);

            if (h.IsCloseTo(360))
                h = 0;
            while (h > 360)
                h -= 360;
            while (h < 0)
                h += 360;

            h /= 60;

            var i = (int)Math.Floor(h);
            var f = h - i;
            var p = b * (1 - s);
            var q = b * (1 - (s * f));
            var t = b * (1 - (s * (1 - f)));

            return i switch
            {
                0 => Color.FromRgb((byte)b, (byte)t, (byte)p),
                1 => Color.FromRgb((byte)q, (byte)b, (byte)p),
                2 => Color.FromRgb((byte)p, (byte)b, (byte)t),
                3 => Color.FromRgb((byte)p, (byte)q, (byte)b),
                4 => Color.FromRgb((byte)t, (byte)p, (byte)b),
                5 => Color.FromRgb((byte)b, (byte)p, (byte)q),
                _ => throw new Exception("Invalid HSB values"),
            };
        }

        private static bool IsCloseTo(this double value, double target, double tolerance = double.Epsilon)
          => Math.Abs(value - target) < tolerance;
    }
}
