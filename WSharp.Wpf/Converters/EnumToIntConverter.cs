using System;
using System.Globalization;
using System.Windows.Data;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class EnumToIntConverter : ATypedValueConverter<Enum, int>
    {
        private static EnumToIntConverter instance;
        public static EnumToIntConverter Instance => instance ?? (instance = new EnumToIntConverter());

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return targetType.IsEnum
                ? Enum.ToObject(targetType, value)
                : Binding.DoNothing;
        }

        protected override bool TInToTOut(Enum tin, object parameter, CultureInfo culture, out int tout)
        {
            tout = System.Convert.ToInt32(tin);
            return true;
        }
    }
}
