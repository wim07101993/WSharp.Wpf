using System.Windows;

namespace WSharp.Wpf.Controls
{
    /// <summary>Internal use only.</summary>
    public class ScaleHost : FrameworkElement
    {
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale),
            typeof(double),
            typeof(ScaleHost),
            new PropertyMetadata(0.0));

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
    }
}
