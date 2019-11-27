using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;


namespace WSharp.Wpf.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        private static ListToStringConverter _instance;
        public static ListToStringConverter Instance => _instance ?? (_instance = new ListToStringConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable enumerable))
                return value.ToString();

            var list = enumerable.Cast<object>().ToList();
            if (list.Count <= 0)
                return null;

            var builder = new StringBuilder();
            foreach (var item in list.Take(list.Count - 1))
                builder.Append(item.ToString()).Append(", ");

            builder.Append(list.Last().ToString());
            return builder.ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
