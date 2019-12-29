using System;
using System.ComponentModel;
using System.Globalization;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Converters
{
    public class SnackbarMessageTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) 
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string s 
                ? new SnackbarMessage { Content = s } 
                : base.ConvertFrom(context, culture, value);
        }
    }
}
