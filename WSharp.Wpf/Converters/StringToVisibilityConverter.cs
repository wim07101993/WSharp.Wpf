using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        private static StringToVisibilityConverter _instance;
        public static StringToVisibilityConverter Instance => _instance ?? (_instance = new StringToVisibilityConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string s && !string.IsNullOrWhiteSpace(s)
                ? Visibility.Visible
                : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
