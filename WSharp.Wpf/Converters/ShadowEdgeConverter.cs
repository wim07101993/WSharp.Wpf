using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ShadowEdgeConverter : ATypedMultiValueConverter<object, Brush>
    {
        private static ShadowEdgeConverter instance;
        public static ShadowEdgeConverter Instance => instance ?? (instance = new ShadowEdgeConverter());

        protected override bool ValidateTin(object[] values, CultureInfo culture, out IList<object> typedValues)
        {
            typedValues = values;
            return values.Length == 4 &&
                values[0] is double width && !double.IsNaN(width) && !double.IsInfinity(width) &&
                values[1] is double height && !double.IsNaN(height) && !double.IsInfinity(height) &&
                values[2] is EShadowDepth && values[3] is EShadowEdges;
        }

        protected override bool TInToTOut(IList<object> tins, object parameter, CultureInfo culture, out Brush tout)
        {
            var width = (double)tins[0];
            var height = (double)tins[1];
            var depth = (EShadowDepth)tins[2];
            var edges = (EShadowEdges)tins[3];

            var shadow = ShadowInfo.GetDropShadow(depth);
            if (shadow == null)
            {
                tout = default;
                return false;
            }

            var radius = shadow.BlurRadius;
            var rect = new Rect(0, 0, width, height);

            if (edges.HasFlag(EShadowEdges.Left))
                rect = new Rect(rect.X - radius, rect.Y, rect.Width + radius, rect.Height);
            if (edges.HasFlag(EShadowEdges.Top))
                rect = new Rect(rect.X, rect.Y - radius, rect.Width, rect.Height + radius);
            if (edges.HasFlag(EShadowEdges.Right))
                rect = new Rect(rect.X, rect.Y, rect.Width + radius, rect.Height);
            if (edges.HasFlag(EShadowEdges.Bottom))
                rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height + radius);

            var size = new GeometryDrawing(new SolidColorBrush(Colors.White), new Pen(), new RectangleGeometry(rect));
            tout = new DrawingBrush(size)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                Viewport = rect,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = rect,
                ViewboxUnits = BrushMappingMode.Absolute
            };
            return true;
        }
    }
}
