using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WSharp.Wpf.Converters
{
    public class ShadowEdgeConverter : IMultiValueConverter
    {
        private static ShadowEdgeConverter _instance;
        public static ShadowEdgeConverter Instance => _instance ?? (_instance = new ShadowEdgeConverter());

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length != 4 ||
                !(values[0] is double width) || !(values[1] is double height) ||
                !(values[2] is EShadowDepth) || !(values[3] is EShadowEdges edges))
                return Binding.DoNothing;

            if (double.IsNaN(width) || double.IsInfinity(width) || double.IsNaN(height) || double.IsInfinity(height))
                return Binding.DoNothing;

            DropShadowEffect dropShadow = ShadowInfo.GetDropShadow((EShadowDepth)values[2]);
            if (dropShadow == null)
                return Binding.DoNothing;

            var blurRadius = dropShadow.BlurRadius;

            var rect = new Rect(0, 0, width, height);

            if (edges.HasFlag(EShadowEdges.Left))
                rect = new Rect(rect.X - blurRadius, rect.Y, rect.Width + blurRadius, rect.Height);
            if (edges.HasFlag(EShadowEdges.Top))
                rect = new Rect(rect.X, rect.Y - blurRadius, rect.Width, rect.Height + blurRadius);
            if (edges.HasFlag(EShadowEdges.Right))
                rect = new Rect(rect.X, rect.Y, rect.Width + blurRadius, rect.Height);
            if (edges.HasFlag(EShadowEdges.Bottom))
                rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height + blurRadius);

            var size = new GeometryDrawing(new SolidColorBrush(Colors.White), new Pen(), new RectangleGeometry(rect));
            return new DrawingBrush(size)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                Viewport = rect,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = rect,
                ViewboxUnits = BrushMappingMode.Absolute
            };
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}