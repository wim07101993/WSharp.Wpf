using System.Collections.Generic;
using System.Globalization;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public sealed class MathMultiConverter : ATypedMultiValueConverter<double, double>
    {
        public EMathOperation Operation { get; set; }

        protected override bool ValidateTin(object[] values, CultureInfo culture, out IList<double> typedValues) 
            => base.ValidateTin(values, culture, out typedValues) && typedValues.Count == 2;

        protected override bool TInToTOut(IList<double> tins, object parameter, CultureInfo culture, out double tout)
        {
            tout = Operation switch
            {
                EMathOperation.Divide => tins[0] / tins[1],
                EMathOperation.Multiply => tins[0] * tins[1],
                EMathOperation.Subtract => tins[0] - tins[1],
                _ => tins[0] + tins[1],// (case MathOperation.Add:)
            };
            return true;
        }
    }
}
