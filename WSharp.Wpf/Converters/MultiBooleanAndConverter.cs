using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class MultiBooleanAndConverter : IMultiValueConverter
    {
        private static MultiBooleanAndConverter _instance;
        public static MultiBooleanAndConverter Instance => _instance ?? (_instance = new MultiBooleanAndConverter());

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values?.All(x => x is bool b && b) == true;

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
