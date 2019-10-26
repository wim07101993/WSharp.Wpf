using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace WSharp.Wpf.NetFramework.Converters
{
    public class ObjectToBooleanConverter : IValueConverter
    {
        public static ObjectToBooleanConverter Instance { get; } = new ObjectToBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
