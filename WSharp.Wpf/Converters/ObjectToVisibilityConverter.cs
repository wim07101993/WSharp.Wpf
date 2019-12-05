using System.Globalization;
using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ObjectToVisibilityConverter : ATypedValueConverter<object, Visibility>
    {
        private static ObjectToVisibilityConverter _instance;
        public static ObjectToVisibilityConverter Instance => _instance ?? (_instance = new ObjectToVisibilityConverter());

        protected override bool ValidateTIn(object value, CultureInfo culture, out object typedValue)
        {
            typedValue = value;
            return true;
        }

        protected override bool TInToTOut(object tin, object parameter, CultureInfo culture, out Visibility tout)
        {
            tout = tin == null
                ? Visibility.Collapsed
                : Visibility.Visible;

            return true;
        }
    }
}
