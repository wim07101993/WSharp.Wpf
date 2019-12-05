using System.Globalization;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ObjectToBooleanConverter : ATypedValueConverter<object, bool>
    {
        private static ObjectToBooleanConverter _instance;
        public static ObjectToBooleanConverter Instance => _instance ?? (_instance = new ObjectToBooleanConverter());

        protected override bool ValidateTIn(object value, CultureInfo culture, out object typedValue)
        {
            typedValue = value;
            return true;
        }

        protected override bool TInToTOut(object tin, object parameter, CultureInfo culture, out bool tout)
        {
            tout = tin != null;
            return true;
        }
    }
}
