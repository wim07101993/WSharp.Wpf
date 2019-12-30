using System.Globalization;
using System.Windows.Media;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class BrushRoundConverter : ATypedValueConverter<SolidColorBrush, Brush>
    {
        private static BrushRoundConverter instance;
        public static BrushRoundConverter Instance => instance ?? (instance = new BrushRoundConverter());

        public Brush HighValue { get; set; } = Brushes.White;

        public Brush LowValue { get; set; } = Brushes.Black;

        protected override bool TInToTOut(SolidColorBrush tin, object parameter, CultureInfo culture, out Brush tout)
        {
            var color = tin.Color;
            var brightness = (0.3 * color.R) + (0.59 * color.G) + (0.11 * color.B);
            tout = brightness < 123 ? LowValue : HighValue;
            return true;
        }
    }
}
