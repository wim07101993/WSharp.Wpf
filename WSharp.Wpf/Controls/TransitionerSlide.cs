using System.Windows;
using WSharp.Wpf.Transitions;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    /// Content control to host the content of an individual page within a <see cref="Transitioner"/>.
    /// </summary>
    public class TransitionerSlide : TransitioningContentBase
    {
        public static RoutedEvent InTransitionFinished = EventManager.RegisterRoutedEvent(
            nameof(InTransitionFinished), 
            RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler),
            typeof(TransitionerSlide));

        public static readonly DependencyProperty TransitionOriginProperty = DependencyProperty.Register(
            nameof(TransitionOrigin), 
            typeof(Point), 
            typeof(Transitioner), 
            new PropertyMetadata(new Point(0.5, 0.5)));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State), 
            typeof(ETransitionerSlideState), 
            typeof(TransitionerSlide), 
            new PropertyMetadata(default(ETransitionerSlideState), new PropertyChangedCallback(StatePropertyChangedCallback)));

        public static readonly DependencyProperty ForwardWipeProperty = DependencyProperty.Register(
            nameof(ForwardWipe),
            typeof(ITransitionWipe),
            typeof(TransitionerSlide), 
            new PropertyMetadata(new CircleWipe()));

        public static readonly DependencyProperty BackwardWipeProperty = DependencyProperty.Register(
            nameof(BackwardWipe), 
            typeof(ITransitionWipe),
            typeof(TransitionerSlide), 
            new PropertyMetadata(new SlideOutWipe()));

        static TransitionerSlide()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionerSlide), new FrameworkPropertyMetadata(typeof(TransitionerSlide)));
        }

        public Point TransitionOrigin
        {
            get => (Point)GetValue(TransitionOriginProperty);
            set => SetValue(TransitionOriginProperty, value);
        }

        public ETransitionerSlideState State
        {
            get => (ETransitionerSlideState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public ITransitionWipe ForwardWipe
        {
            get => (ITransitionWipe)GetValue(ForwardWipeProperty);
            set => SetValue(ForwardWipeProperty, value);
        }

        public ITransitionWipe BackwardWipe
        {
            get => (ITransitionWipe)GetValue(BackwardWipeProperty);
            set => SetValue(BackwardWipeProperty, value);
        }

        protected void OnInTransitionFinished(RoutedEventArgs e) => RaiseEvent(e);

        private void AnimateToState()
        {
            if (State != ETransitionerSlideState.Current)
                return;

            RunOpeningEffects();
        }
  
        private static void StatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TransitionerSlide transitionerSlide))
                return;

            transitionerSlide.AnimateToState();
        }
    }
}
