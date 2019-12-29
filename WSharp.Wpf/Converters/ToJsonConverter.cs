using System;
using System.Globalization;
using System.Windows.Data;

using WSharp.Extensions;

namespace WSharp.Wpf.Converters
{
    public class ToJsonConverter : IValueConverter
    {
        private static ToJsonConverter instance;
        public static ToJsonConverter Instance => instance ?? (instance = new ToJsonConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.SerializeJson();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString().DeserializeJson(targetType);
    }
}
