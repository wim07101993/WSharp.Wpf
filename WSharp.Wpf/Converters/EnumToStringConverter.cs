using System;
using System.Globalization;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class EnumToStringConverter : ATypedValueConverter<Enum, string>
    {
        private static EnumToStringConverter instance;
        public static EnumToStringConverter Instance => instance ??= new EnumToStringConverter();

        protected override bool TInToTOut(Enum tin, object parameter, CultureInfo culture, out string tout)
        {
            tout = tin.ToString();
            return true;
        }

        protected override bool TOutToTIn(string tout, object parameter, CultureInfo culture, out Enum tin)
        {
            if (!(parameter is Type type))
            {
                tin = default;
                return false;
            }

            tin = (Enum)Enum.Parse(type, tout, false);
            return true;
        }
    }
}
