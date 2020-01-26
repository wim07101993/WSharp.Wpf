using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class AMultiBooleanAndConverter<T> : ATypedMultiValueConverter<bool, T>
    {
        public abstract T TrueValue { get; }
        public abstract T FalseValue { get; }

        protected override bool TInToTOut(IList<bool> tins, object parameter, CultureInfo culture, out T tout)
        {
            tout = tins?.All(x => x) == true
                ? TrueValue
                : FalseValue;

            return true;
        }

        protected override bool TOutToTIn(T tout, Type[] targetTypes, object parameter, CultureInfo culture, out IEnumerable<bool> tins)
        {
            var equals = Equals(tout, TrueValue);
            tins = targetTypes.Select(x => equals).ToList();
            return true;
        }
    }
}
