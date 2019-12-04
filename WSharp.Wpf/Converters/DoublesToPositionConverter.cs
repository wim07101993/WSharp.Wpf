using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public class DoublesToPositionConverter : IMultiValueConverter
    {
        private static DoublesToPositionConverter _instance;
        public static DoublesToPositionConverter Instance => _instance ?? (_instance = new DoublesToPositionConverter());

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3 || values.Any(v => v == null))
                return Binding.DoNothing;

            if (!double.TryParse(values[0].ToString(), out var positionAsScaleFactor) || 
                !double.TryParse(values[1].ToString(), out var lower) || 
                !double.TryParse(values[2].ToString(), out var upper))
                return Binding.DoNothing;

            return upper + (lower - upper) * positionAsScaleFactor; ;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
