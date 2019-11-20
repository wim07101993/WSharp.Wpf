using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class NewLineRemover : IValueConverter
    {
        private static NewLineRemover _instance;
        public static NewLineRemover Instance => _instance ?? (_instance = new NewLineRemover());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var str = value.ToString()
                .Replace('\r', ',')
                .Replace('\n', ' ')
                .Replace('\t', ' ')
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(" ", str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
