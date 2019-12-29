using System.Collections;
using System.Globalization;
using System.Windows;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ListToVisibilityConverter : ATypedValueConverter<ICollection, Visibility>
    {
        private static ListToVisibilityConverter instance;
        public static ListToVisibilityConverter Instance => instance ?? (instance = new ListToVisibilityConverter());

        protected override bool ValidateTIn(object value, CultureInfo culture, out ICollection typedValue)
            => base.ValidateTIn(value, culture, out typedValue) || value == null;

        protected override bool TInToTOut(ICollection tin, object parameter, CultureInfo culture, out Visibility tout)
        {
            tout = tin != null && tin.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            return true;
        }
    }
}
