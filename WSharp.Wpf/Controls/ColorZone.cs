using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{
    /// <summary>
    ///     User a colour zone to easily switch the background and foreground colours, whilst still
    ///     remaining within the selected Material Design palette.
    /// </summary>
    public class ColorZone : ContentControl
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            nameof(Mode), 
            typeof(EColorZoneMode), 
            typeof(ColorZone), 
            new PropertyMetadata(default(EColorZoneMode)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius), 
            typeof(CornerRadius), 
            typeof(ColorZone), 
            new PropertyMetadata(default(CornerRadius)));

        static ColorZone()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorZone), new FrameworkPropertyMetadata(typeof(ColorZone)));
        }

        public EColorZoneMode Mode
        {
            get => (EColorZoneMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
