using System;
using System.Globalization;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class InvertedBooleanConverter : IValueConverter
    {
        private static InvertedBooleanConverter _instance;
        public static InvertedBooleanConverter Instance => _instance ?? (_instance = new InvertedBooleanConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;
    }
}
