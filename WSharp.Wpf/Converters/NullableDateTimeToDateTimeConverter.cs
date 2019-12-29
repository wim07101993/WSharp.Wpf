using System;
using System.Globalization;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class NullableDateTimeToDateTimeConverter : IValueConverter
    {
        private static NullableDateTimeToDateTimeConverter instance;
        public static NullableDateTimeToDateTimeConverter Instance => instance ?? (instance = new NullableDateTimeToDateTimeConverter());

        public DateTime Default { get; set; } = DateTime.Now;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is DateTime ? value : Default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
