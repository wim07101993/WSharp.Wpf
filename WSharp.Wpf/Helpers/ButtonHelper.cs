using System.Windows;

namespace WSharp.Wpf.Helpers
{
    public static class ButtonHelper
    {
        #region CornerRadius

        /// <summary>Controls the corner radius of the surrounding box.</summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(ButtonHelper),
            new PropertyMetadata(new CornerRadius(2.0)));

        public static void SetCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

        public static CornerRadius GetCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(CornerRadiusProperty);

        #endregion CornerRadius
    }
}
