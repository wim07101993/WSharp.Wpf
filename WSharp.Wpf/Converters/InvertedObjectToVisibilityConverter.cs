using System.Globalization;
using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class InvertedObjectToVisibilityConverter : ATypedValueConverter<object, Visibility>
    {
        private static InvertedObjectToVisibilityConverter instance;
        public static InvertedObjectToVisibilityConverter Instance => instance ??= new InvertedObjectToVisibilityConverter();

        protected override bool TInToTOut(object tin, object parameter, CultureInfo culture, out Visibility tout)
        {
            tout = tin == null
                ? Visibility.Visible
                : Visibility.Collapsed;
            return true;
        }
    }
}
