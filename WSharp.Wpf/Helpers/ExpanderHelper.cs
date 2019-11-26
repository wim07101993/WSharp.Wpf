using System.Windows;

namespace WSharp.Wpf.Helpers
{
    public static class ExpanderHelper
    {
        #region left header padding

        public static readonly DependencyProperty LeftHeaderPaddingProperty = DependencyProperty.RegisterAttached(
                "LeftHeaderPadding",
                typeof(Thickness),
                typeof(ExpanderHelper),
                new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetLeftHeaderPadding(DependencyObject element) => (Thickness)element.GetValue(LeftHeaderPaddingProperty);

        public static void SetLeftHeaderPadding(DependencyObject element, Thickness value) => element.SetValue(LeftHeaderPaddingProperty, value);

        #endregion left header padding

        #region right header padding

        public static readonly DependencyProperty RightHeaderPaddingProperty = DependencyProperty.RegisterAttached(
                "RightHeaderPadding",
                typeof(Thickness),
                typeof(ExpanderHelper),
                new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetRightHeaderPadding(DependencyObject element) => (Thickness)element.GetValue(RightHeaderPaddingProperty);

        public static void SetRightHeaderPadding(DependencyObject element, Thickness value) => element.SetValue(RightHeaderPaddingProperty, value);

        #endregion right header padding

        #region up header padding

        public static readonly DependencyProperty UpHeaderPaddingProperty = DependencyProperty.RegisterAttached(
                "UpHeaderPadding",
                typeof(Thickness),
                typeof(ExpanderHelper),
                new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetUpHeaderPadding(DependencyObject element) => (Thickness)element.GetValue(UpHeaderPaddingProperty);

        public static void SetUpHeaderPadding(DependencyObject element, Thickness value) => element.SetValue(UpHeaderPaddingProperty, value);

        #endregion up header padding

        #region down header padding

        public static readonly DependencyProperty DownHeaderPaddingProperty = DependencyProperty.RegisterAttached(
                "DownHeaderPadding",
                typeof(Thickness),
                typeof(ExpanderHelper),
                new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetDownHeaderPadding(DependencyObject element) => (Thickness)element.GetValue(DownHeaderPaddingProperty);

        public static void SetDownHeaderPadding(DependencyObject element, Thickness value) => element.SetValue(DownHeaderPaddingProperty, value);

        #endregion down header padding
    }
}