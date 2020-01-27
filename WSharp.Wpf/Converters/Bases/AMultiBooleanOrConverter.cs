using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WSharp.Wpf.Converters.Bases
{
    public abstract class AMultiBooleanOrConverter<T> : ATypedMultiValueConverter<bool, T>
    {
        public abstract T TrueValue { get; }
        public abstract T FalseValue { get; }

        protected override bool TInToTOut(IList<bool> tins, object parameter, CultureInfo culture, out T tout)
        {
            tout = tins?.Any(x => x) == true
                ? TrueValue
                : FalseValue;

            return true;
        }
    }
}
