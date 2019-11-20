using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        private static InvertedBooleanToVisibilityConverter _instance;
        public static InvertedBooleanToVisibilityConverter Instance => _instance ?? (_instance = new InvertedBooleanToVisibilityConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b
                ? Visibility.Collapsed
                : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility v && v == Visibility.Collapsed;
    }
}
