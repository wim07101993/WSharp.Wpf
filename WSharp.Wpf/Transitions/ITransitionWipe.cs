using System.Windows;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Transitions
{
    public interface ITransitionWipe
    {
        void Wipe(TransitionerSlide fromSlide, TransitionerSlide toSlide, Point origin, IZIndexController zIndexController);
    }
}
