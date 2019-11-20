using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private static BooleanToVisibilityConverter _instance;
        public static BooleanToVisibilityConverter Instance => _instance ?? (_instance = new BooleanToVisibilityConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b
                ? Visibility.Visible
                : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility v && v == Visibility.Visible;
    }
}
