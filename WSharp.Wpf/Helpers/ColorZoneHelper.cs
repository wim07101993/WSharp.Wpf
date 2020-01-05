using System.Windows;
using WSharp.Wpf.Controls;

namespace WSharp.Wpf.Helpers
{
    public static class ColorZoneHelper
    {
        #region Mode

        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode", 
            typeof(EColorZoneMode),
            typeof(ColorZoneHelper), 
            new FrameworkPropertyMetadata(default(EColorZoneMode), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMode(DependencyObject element, EColorZoneMode value) => element.SetValue(ModeProperty, value);

        public static EColorZoneMode GetMode(DependencyObject element) => (EColorZoneMode)element.GetValue(ModeProperty);

        #endregion Mode
    }
}
