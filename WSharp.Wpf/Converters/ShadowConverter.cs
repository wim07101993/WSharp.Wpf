using System.Globalization;
using System.Windows.Media.Effects;

using WSharp.Wpf.Converters.Bases;

namespace WSharp.Wpf.Converters
{
    public class ShadowConverter : ATypedValueConverter<EShadowDepth, DropShadowEffect>
    {
        private static ShadowConverter instance;
        public static ShadowConverter Instance => instance ??= new ShadowConverter();

        protected override bool TInToTOut(EShadowDepth tin, object parameter, CultureInfo culture, out DropShadowEffect tout)
        {
            tout = Clone(Convert(tin));
            return true;
        }

        public static DropShadowEffect Convert(EShadowDepth shadowDepth) => ShadowInfo.GetDropShadow(shadowDepth);

        private static DropShadowEffect Clone(DropShadowEffect dropShadowEffect)
        {
            return dropShadowEffect == null
                ? null
                : new DropShadowEffect()
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
