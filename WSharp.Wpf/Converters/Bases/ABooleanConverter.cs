using System.Globalization;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class ABooleanConverter<T> : ATypedValueConverter<bool, T>
    {
        public abstract T TrueValue { get; }
        public abstract T FalseValue { get; }

        protected override bool TInToTOut(bool tin, object parameter, CultureInfo culture, out T tout)
        {
            tout = tin ? TrueValue : FalseValue;
            return true;
        }

        protected override bool TOutToTIn(T tout, object parameter, CultureInfo culture, out bool tin)
        {
            tin = Equals(tout, TrueValue);
            return true;
        }
    }
}
