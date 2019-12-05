using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class ATypedMultiValueConverter<TIn, TOut> : IMultiValueConverter
    {
        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return ValidateTin(values, culture, out var tin) && TInToTOut(tin, parameter, culture, out var tout)
                ? tout
                : Binding.DoNothing;
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return ValidateTOut(value, culture, out var typedValue) && TOutToTIn(typedValue, targetTypes, parameter, culture, out var tins)
                ? tins.Cast<object>().ToArray()
                : throw new NotImplementedException();
        }


        protected virtual bool ValidateTin(object[] values, CultureInfo culture, out IList<TIn> typedValues)
        {
            typedValues = values?.OfType<TIn>().ToList();
            return typedValues != null && typedValues.Count == values.Length;
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

        protected abstract bool TInToTOut(IList<TIn> tins, object parameter, CultureInfo culture, out TOut tout);

        protected virtual bool TOutToTIn(TOut tout, Type[] targetTypes, object parameter, CultureInfo culture, out IEnumerable<TIn> tins)
        {
            tins = default;
            return false;
        }
    }
}
