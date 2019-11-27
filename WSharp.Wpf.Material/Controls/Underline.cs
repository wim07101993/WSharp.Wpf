using System.Windows;
using System.Windows.Controls;
using WSharp.Wpf.Helpers;

namespace WSharp.Wpf.Material.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(GroupName = "ActivationStates", Name = ActiveStateName)]
    [TemplateVisualState(GroupName = "ActivationStates", Name = InactiveStateName)]
    public class Underline : Control
    {
        public const string ActiveStateName = "Active";
        public const string InactiveStateName = "Inactive";

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), 
            typeof(bool), 
            typeof(Underline),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, IsActivePropertyChangedCallback));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), 
            typeof(CornerRadius), 
            typeof(Underline),
            new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.AffectsRender, null));

        static Underline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoVisualState(false);
        }

        private void GotoVisualState(bool useTransitions) => VisualStateManager.GoToState(this, SelectStateName(), useTransitions);

        private string SelectStateName() => IsActive ? ActiveStateName : InactiveStateName;

        private static void IsActivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (!(d is Underline underline))
                return;

            underline.GotoVisualState(!TransitionHelper.GetDisableTransitions(d));
        }
    }
}