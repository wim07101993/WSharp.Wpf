using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace WSharp.Wpf.Helpers
{
    internal static class DpiHelper
    {
        private static readonly int dpiX;
        private static readonly int dpiY;

        private const double StandardDpiX = 96.0;
        private const double StandardDpiY = 96.0;

        static DpiHelper()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            dpiX = (int)dpiXProperty.GetValue(null, null);
            dpiY = (int)dpiYProperty.GetValue(null, null);
        }

        public static double TransformToDeviceY(Visual visual, double y)
        {
            var source = PresentationSource.FromVisual(visual);

            return source?.CompositionTarget != null 
                ? y * source.CompositionTarget.TransformToDevice.M22 
                : TransformToDeviceY(y);
        }

        public static double TransformToDeviceX(Visual visual, double x)
        {
            var source = PresentationSource.FromVisual(visual);

            return source?.CompositionTarget != null 
                ? x * source.CompositionTarget.TransformToDevice.M11 
                : TransformToDeviceX(x);
        }

        public static double TransformToDeviceY(double y) => y * dpiY / StandardDpiY;

        public static double TransformToDeviceX(double x) => x * dpiX / StandardDpiX;
    }
}
