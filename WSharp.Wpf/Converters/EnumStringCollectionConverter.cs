using System;
using System.Globalization;
using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class EnumStringCollectionConverter : ATypedValueConverter<Type, string[]>
    {
        private static EnumStringCollectionConverter _instance;
        public static EnumStringCollectionConverter Instance => _instance ?? (_instance = new EnumStringCollectionConverter());

        protected override bool ValidateTIn(object value, CultureInfo culture, out Type typedValue)
        {
            typedValue = value?.GetType();
            return typedValue != null && typedValue.IsEnum;
        }

        protected override bool TInToTOut(Type tin, object parameter, CultureInfo culture, out string[] tout)
        {
            tout = Enum.GetNames(tin);
            return true;
        }
    }
}
