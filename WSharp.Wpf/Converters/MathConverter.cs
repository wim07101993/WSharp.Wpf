using System;
using System.Globalization;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public sealed class MathConverter : ATypedValueConverter<double, double>
    {
        public EMathOperation Operation { get; set; }

        protected override bool ValidateTIn(object value, CultureInfo culture, out double typedValue)
        {
            try
            {
                typedValue = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                typedValue = default;
                return false;
            }
        }

        protected override bool TInToTOut(double tin, object parameter, CultureInfo culture, out double tout)
        {
            try
            {
                var param = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);

                switch (Operation)
                {
                    case EMathOperation.Add:
                        tout = tin + param;
                        return true;
                    case EMathOperation.Divide:
                        tout = tin / param;
                        return true;
                    case EMathOperation.Multiply:
                        tout = tin * param;
                        return true;
                    case EMathOperation.Subtract:
                        tout = tin - param;
                        return true;
                }
            }
            catch (Exception)
            {
            }

            tout = default;
            return false;
        }
    }
}
