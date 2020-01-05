using System.Windows;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    /// Content control to enable easier transitions.
    /// </summary>
    public class TransitioningContent : TransitioningContentBase
    {
        public static readonly DependencyProperty RunHintProperty = DependencyProperty.Register(
            nameof(RunHint),
            typeof(ETransitioningContentRunHint), 
            typeof(TransitioningContent), 
            new PropertyMetadata(ETransitioningContentRunHint.All));

        static TransitioningContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningContent), new FrameworkPropertyMetadata(typeof(TransitioningContent)));
        }

        public TransitioningContent()
        {
            Loaded += (sender, args) => Run(ETransitioningContentRunHint.Loaded);
            IsVisibleChanged += (sender, args) => Run(ETransitioningContentRunHint.IsVisibleChanged);
            
        }

        public ETransitioningContentRunHint RunHint
        {
            get => (ETransitioningContentRunHint)GetValue(RunHintProperty);
            set => SetValue(RunHintProperty, value);
        }

        private void Run(ETransitioningContentRunHint requiredHint)
        {
            if ((RunHint & requiredHint) == requiredHint)
                RunOpeningEffects();
        }
    }
}
