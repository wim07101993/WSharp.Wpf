using System.Globalization;
using System.Windows.Media;
using WSharp.Wpf.Colors;
using WSharp.Wpf.Converters.Bases;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Converters
{
    public class HsbLinearGradientConverter : ATypedValueConverter<double, LinearGradientBrush>
    {
        private static HsbLinearGradientConverter instance;
        public static HsbLinearGradientConverter Instance => instance ??= new HsbLinearGradientConverter();

        protected override bool TInToTOut(double tin, object parameter, CultureInfo culture, out LinearGradientBrush tout)
        {
            tout = new LinearGradientBrush(System.Windows.Media.Colors.White, new Hsb(tin, 1, 1).ToColor(), 0);
            return true;
        }
    }
}
