using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ListToNoNewLineStringConverter : ATypedValueConverter<IList<object>, string>
    {
        private static ListToNoNewLineStringConverter instance;
        public static ListToNoNewLineStringConverter Instance => instance ?? (instance = new ListToNoNewLineStringConverter());

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
            var split = tin
                .Take(tin.Count - 1)
                .Aggregate(new StringBuilder(), (builder, o) => builder.Append($"{o ?? "null"}, "))
                .Append(tin.Last().ToString())
                .ToString()
                .Replace('\r', ',')
                .Replace('\n', ' ')
                .Replace('\t', ' ')
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            tout = string.Join(" ", split);
            return true;
        }
    }
}
