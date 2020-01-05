using System;
using System.Windows.Markup;
using WSharp.Wpf.Transitions;

namespace WSharp.Wpf.Extensions
{
    [MarkupExtensionReturnType(typeof(TransitionEffectBase))]
    public class TransitionEffectExtension : MarkupExtension
    {
        public TransitionEffectExtension()
        {
            Kind = ETransitionEffectKind.None;
        }

        public TransitionEffectExtension(ETransitionEffectKind kind)
        {
            Kind = kind;
        }

        public TransitionEffectExtension(ETransitionEffectKind kind, TimeSpan duration)
        {
            Kind = kind;
            Duration = duration;
        }

        [ConstructorArgument("kind")]
        public ETransitionEffectKind Kind { get; set; }

        [ConstructorArgument("duration")]
        public TimeSpan? Duration { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Duration.HasValue ? new TransitionEffect(Kind, Duration.Value) : new TransitionEffect(Kind);
        }
    }
}
