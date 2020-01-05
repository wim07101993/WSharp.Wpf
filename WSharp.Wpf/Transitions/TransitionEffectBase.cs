using System.Windows;
using System.Windows.Media.Animation;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Transitions
{
    public abstract class TransitionEffectBase : FrameworkElement, ITransitionEffect
    {
        public abstract Timeline Build<TSubject>(TSubject effectSubject) where TSubject : FrameworkElement, ITransitionEffectSubject;        
    }
}
