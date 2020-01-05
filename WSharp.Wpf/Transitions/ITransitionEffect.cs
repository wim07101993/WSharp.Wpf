using System.Windows;
using System.Windows.Media.Animation;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Transitions
{
    public interface ITransitionEffect
    {
        Timeline Build<TSubject>(TSubject effectSubject) where TSubject : FrameworkElement, ITransitionEffectSubject;
    }
}
