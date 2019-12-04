using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndToVisibilityConverter : IMultiValueConverter
    {
        private static MultiBooleanAndToVisibilityConverter _instance;
        public static MultiBooleanAndToVisibilityConverter Instance => _instance ?? (_instance = new MultiBooleanAndToVisibilityConverter());

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return false;

            return values.All(x => x is bool b && b) 
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
