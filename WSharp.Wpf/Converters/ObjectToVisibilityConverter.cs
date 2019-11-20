using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        private static ObjectToVisibilityConverter _instance;
        public static ObjectToVisibilityConverter Instance => _instance ?? (_instance = new ObjectToVisibilityConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
