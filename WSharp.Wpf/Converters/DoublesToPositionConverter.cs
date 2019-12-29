using System.Collections.Generic;
using System.Globalization;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class DoublesToPositionConverter : ATypedMultiValueConverter<double, double>
    {
        private static DoublesToPositionConverter instance;
        public static DoublesToPositionConverter Instance => instance ?? (instance = new DoublesToPositionConverter());

        protected override bool ValidateTin(object[] values, CultureInfo culture, out IList<double> typedValues)
        {
            return base.ValidateTin(values, culture, out typedValues) && typedValues.Count == 3;
        }

        protected override bool TInToTOut(IList<double> tins, object parameter, CultureInfo culture, out double tout)
        {
            var positionAsScaleFactor = tins[0];
            var lower = tins[1];
            var upper = tins[2];

            tout = upper + (lower - upper) * positionAsScaleFactor;
            return true;
        }
    }
}
