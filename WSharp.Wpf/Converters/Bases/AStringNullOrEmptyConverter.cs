using System.Globalization;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class AStringNullOrEmptyConverter<T> : ATypedValueConverter<string, T>
    {
        public abstract T NullOrEmptyValue { get; }
        public abstract T NotNullOrEmptyValue { get; }

        protected override bool ValidateTIn(object value, CultureInfo culture, out string typedValue)
        {
            if (base.ValidateTIn(value, culture, out typedValue))
                return true;

            typedValue = value?.ToString();
            return true;
        }

        protected override bool TInToTOut(string tin, object parameter, CultureInfo culture, out T tout)
        {
            tout = string.IsNullOrEmpty(tin)
                ? NullOrEmptyValue
                : NotNullOrEmptyValue;

            return true;
        }
    }
}
