using System.Windows;
using System.Windows.Controls;

namespace MaterialDesignThemes.Wpf
{
    internal static class ScrollViewerHelper
    {
        #region SyncHorizontalOffset

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
            "SyncHorizontalOffset", 
            typeof(double), 
            typeof(ScrollViewerHelper),
            new PropertyMetadata(default(double), OnSyncHorizontalOffsetChanged));

        public static void SetSyncHorizontalOffset(DependencyObject element, double value) 
            => element.SetValue(HorizontalOffsetProperty, value);

        public static double GetSyncHorizontalOffset(DependencyObject element) 
            => (double)element.GetValue(HorizontalOffsetProperty);

        private static void OnSyncHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer))
                return;

            scrollViewer?.ScrollToHorizontalOffset((double)e.NewValue);
        }

        #endregion SyncHorizontalOffset
    }
}
