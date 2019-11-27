using System;
using System.Globalization;
using System.Windows.Data;

namespace WSharp.Wpf.Converters
{
    public sealed class MathConverter : IValueConverter
    {
        public EMathOperation Operation { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var value1 = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                var value2 = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
                switch (Operation)
                {
                    case EMathOperation.Add:
                        return value1 + value2;
                    case EMathOperation.Divide:
                        return value1 / value2;
                    case EMathOperation.Multiply:
                        return value1 * value2;
                    case EMathOperation.Subtract:
                        return value1 - value2;
                    default:
                        return Binding.DoNothing;
                }
            }
            catch (FormatException)
            {
                return Binding.DoNothing;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
