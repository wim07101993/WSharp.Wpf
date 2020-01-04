using System;
using System.Globalization;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class NewLineRemover : ATypedValueConverter<string, string>
    {
        private static NewLineRemover instance;
        public static NewLineRemover Instance => instance ??= new NewLineRemover();

        protected override bool ValidateTIn(object value, CultureInfo culture, out string typedValue)
        {
            if (base.ValidateTIn(value, culture, out typedValue))
                return true;

            typedValue = value?.ToString();
            return typedValue != null;
        }

        protected override bool TInToTOut(string tin, object parameter, CultureInfo culture, out string tout)
        {
            var split = tin
                .Replace('\r', ',')
                .Replace('\n', ' ')
                .Replace('\t', ' ')
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            tout = string.Join(" ", split);
            return true;
        }
    }
}
