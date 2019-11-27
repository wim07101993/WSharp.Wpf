using System;
using System.Globalization;
using System.Windows.Data;

namespace WSharp.Wpf.Controls
{
    public class NullableDateTimeToDateTimeConverter : IValueConverter
    {
        private static  NullableDateTimeToDateTimeConverter _instance;
        public static NullableDateTimeToDateTimeConverter Instance => _instance ?? (_instance = new NullableDateTimeToDateTimeConverter());

        public DateTime Default { get; set; } = DateTime.Now;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is DateTime ? value : Default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
