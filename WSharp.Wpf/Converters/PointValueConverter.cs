using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class PointValueConverter : ATypedMultiValueConverter<object, Point>
    {
        private static PointValueConverter instance;
        public static PointValueConverter Instance => instance ??= new PointValueConverter();

        protected override bool ValidateTin(object[] values, CultureInfo culture, out IList<object> typedValues)
            => base.ValidateTin(values, culture, out typedValues) && values?.Length == 2 && values[0] != null && values[1] != null;

        protected override bool TInToTOut(IList<object> tins, object parameter, CultureInfo culture, out Point tout)
        {
            if (double.TryParse(tins[0].ToString(), out var x) &&
                double.TryParse(tins[1].ToString(), out var y))
            {
                tout = new Point(x, y);
                return true;
            }

            tout = default;
            return false;
        }

        protected override bool TOutToTIn(Point tout, Type[] targetTypes, object parameter, CultureInfo culture, out IEnumerable<object> tins)
        {
            tins = new object[] { tout.X, tout.Y };
            return true;
        }
    }
}
