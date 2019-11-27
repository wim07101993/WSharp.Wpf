using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class LabelProxyConverter : IValueConverter
    {
        private static LabelProxyConverter _instance;

        public static LabelProxyConverter Instance => _instance ?? (_instance = new LabelProxyConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => LabelProxyProvider.Get(value as Control);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
