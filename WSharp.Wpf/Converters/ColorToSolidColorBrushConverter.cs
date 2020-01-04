using System.Globalization;
using System.Windows.Media;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ColorToSolidColorBrushConverter : ATypedValueConverter<Color, SolidColorBrush>
    {
        private static ColorToSolidColorBrushConverter instance;
        public static ColorToSolidColorBrushConverter Instance => instance ??= new ColorToSolidColorBrushConverter();

        protected override bool TInToTOut(Color tin, object parameter, CultureInfo culture, out SolidColorBrush tout)
        {
            tout = new SolidColorBrush(tin);
            return true;
        }

        protected override bool TOutToTIn(SolidColorBrush tout, object parameter, CultureInfo culture, out Color tin)
        {
            tin = tout.Color;
            return true;
        }
    }
}
