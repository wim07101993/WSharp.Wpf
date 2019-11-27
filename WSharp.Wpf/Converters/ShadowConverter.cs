using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace WSharp.Wpf.Converters
{
    public class ShadowConverter : IValueConverter
    {
        private static ShadowConverter _instance;
        public static ShadowConverter Instance => _instance ?? (_instance = new ShadowConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EShadowDepth)) 
                return null;

            return Clone(Convert((EShadowDepth)value));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public static DropShadowEffect Convert(EShadowDepth shadowDepth) => ShadowInfo.GetDropShadow(shadowDepth);

        private static DropShadowEffect Clone(DropShadowEffect dropShadowEffect)
        {
            if (dropShadowEffect == null) return null;
            return new DropShadowEffect()
            {
                BlurRadius = dropShadowEffect.BlurRadius,
                Color = dropShadowEffect.Color,
                Direction = dropShadowEffect.Direction,
                Opacity = dropShadowEffect.Opacity,
                RenderingBias = dropShadowEffect.RenderingBias,
                ShadowDepth = dropShadowEffect.ShadowDepth
            };
        }
    }
}
