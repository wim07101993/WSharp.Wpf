using System.Collections.Generic;
using System.Globalization;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public sealed class MathMultiConverter : ATypedMultiValueConverter<double, double>
    {
        public EMathOperation Operation { get; set; }

        protected override bool ValidateTin(object[] values, CultureInfo culture, out IList<double> typedValues)
        {
            return base.ValidateTin(values, culture, out typedValues) && typedValues.Count == 2;
        }

        protected override bool TInToTOut(IList<double> tins, object parameter, CultureInfo culture, out double tout)
        {
            switch (Operation)
            {
                case EMathOperation.Divide:
                    tout = tins[0] / tins[1];
                    break;

                case EMathOperation.Multiply:
                    tout = tins[0] * tins[1];
                    break;

                case EMathOperation.Subtract:
                    tout = tins[0] - tins[1];
                    break;

                default:
                    // (case MathOperation.Add:)
                    tout = tins[0] + tins[1];
                    break;
            }

            return true;
        }
    }
}
