using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ListToStringConverter : ATypedValueConverter<IList<object>, string>
    {
        private static ListToStringConverter instance;
        public static ListToStringConverter Instance => instance ?? (instance = new ListToStringConverter());

        protected override bool ValidateTIn(object value, CultureInfo culture, out IList<object> typedValue)
        {
            if (!(value is IEnumerable enumerable))
            {
                typedValue = default;
                return false;
            }

            typedValue = enumerable.Cast<object>().ToList();
            return typedValue.Count > 0;
        }

        protected override bool TInToTOut(IList<object> tin, object parameter, CultureInfo culture, out string tout)
        {
            tout = tin
                .Take(tin.Count - 1)
                .Aggregate(new StringBuilder(), (builder, o) => builder.Append($"{o ?? "null"}, "))
                .Append(tin.Last().ToString())
                .ToString();
            return true;
        }
    }
}
