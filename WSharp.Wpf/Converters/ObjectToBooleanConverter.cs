using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace WSharp.Wpf.Converters
{
    public class ObjectToBooleanConverter : IValueConverter
    {
        private static ObjectToBooleanConverter _instance;
        public static ObjectToBooleanConverter Instance => _instance ?? (_instance = new ObjectToBooleanConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
