using System.Windows;

namespace MaterialDesignThemes.Wpf
{
    public static class ListViewHelper
    {
        #region ListViewItemPadding

        public static readonly DependencyProperty ListViewItemPaddingProperty = DependencyProperty.RegisterAttached(
            "ListViewItemPadding",
            typeof(Thickness),
            typeof(ListViewHelper),
            new FrameworkPropertyMetadata(new Thickness(8, 8, 8, 8), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetListViewItemPadding(DependencyObject element, Thickness value)
            => element.SetValue(ListViewItemPaddingProperty, value);

        public static Thickness GetListViewItemPadding(DependencyObject element)
            => (Thickness)element.GetValue(ListViewItemPaddingProperty);

        #endregion ListViewItemPadding
    }
}
