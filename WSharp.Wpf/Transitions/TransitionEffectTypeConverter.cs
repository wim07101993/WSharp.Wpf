using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace WSharp.Wpf.Transitions
{
    public class TransitionEffectTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) 
            => sourceType == typeof(string) || typeof(ETransitionEffectKind).IsAssignableFrom(sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var transitionEffect = value is string stringValue &&
                Enum.TryParse(stringValue, out ETransitionEffectKind effectKind)
                ? new TransitionEffect(effectKind)
                : value as TransitionEffectBase;

            if (transitionEffect == null)
                throw new XamlParseException($"Could not parse to type {typeof(ETransitionEffectKind).FullName} or {typeof(TransitionEffectBase).FullName}.");

            return transitionEffect;
        }
    }
}
