using System;
using System.Globalization;
using System.Windows.Data;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class ATypedValueConverter<TIn, TOut> : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValidateTIn(value, culture, out var tin) && TInToTOut(tin, parameter, culture, out var tout)
                ? tout
                : Binding.DoNothing;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValidateTOut(value, culture, out var tout) && TOutToTIn(tout, parameter, culture, out var tin)
                ? tin
                : Binding.DoNothing;
        }

        protected virtual bool ValidateTIn(object value, CultureInfo culture, out TIn typedValue)
        {
            if (!(value is TIn t))
            {
                typedValue = default;
                return false;
            }

            typedValue = t;
            return true;
        }

        protected virtual bool ValidateTOut(object value, CultureInfo culture, out TOut typedValue)
        {
            if (!(value is TOut t))
            {
                typedValue = default;
                return false;
            }

            typedValue = t;
            return true;
        }

        protected abstract bool TInToTOut(TIn tin, object parameter, CultureInfo culture, out TOut tout);
        protected virtual bool TOutToTIn(TOut tout, object parameter, CultureInfo culture, out TIn tin)
        {
            tin = default;
            return false;
        }
    }
}
