using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WSharp.Wpf.Helpers
{
    public static class ValidationHelper
    {
        #region only show on focus

        /// <summary>The hint property</summary>
        public static readonly DependencyProperty OnlyShowOnFocusProperty = DependencyProperty.RegisterAttached(
            "OnlyShowOnFocus",
            typeof(bool),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetOnlyShowOnFocus(DependencyObject element) => (bool)element.GetValue(OnlyShowOnFocusProperty);

        public static void SetOnlyShowOnFocus(DependencyObject element, bool value) => element.SetValue(OnlyShowOnFocusProperty, value);

        #endregion only show on focus

        #region use popup

        /// <summary>The hint property</summary>
        public static readonly DependencyProperty UsePopupProperty = DependencyProperty.RegisterAttached(
            "UsePopup",
            typeof(bool),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetUsePopup(DependencyObject element) => (bool)element.GetValue(UsePopupProperty);

        public static void SetUsePopup(DependencyObject element, bool value) => element.SetValue(UsePopupProperty, value);

        #endregion use popup

        #region popup placement

        /// <summary>The hint property</summary>
        public static readonly DependencyProperty PopupPlacementProperty = DependencyProperty.RegisterAttached(
            "PopupPlacement",
            typeof(PlacementMode),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(PlacementMode.Bottom, FrameworkPropertyMetadataOptions.Inherits));

        public static PlacementMode GetPopupPlacement(DependencyObject element) => (PlacementMode)element.GetValue(PopupPlacementProperty);

        public static void SetPopupPlacement(DependencyObject element, PlacementMode value) => element.SetValue(PopupPlacementProperty, value);

        #endregion popup placement

        #region suppress

        /// <summary>Framework use only.</summary>
        public static readonly DependencyProperty SuppressProperty = DependencyProperty.RegisterAttached(
            "Suppress",
            typeof(bool),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Framework use only.</summary>
        public static bool GetSuppress(DependencyObject element) => (bool)element.GetValue(SuppressProperty);

        /// <summary>Framework use only.</summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetSuppress(DependencyObject element, bool value) => element.SetValue(SuppressProperty, value);

        #endregion suppress

        #region background

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
            "Background", 
            typeof(Brush), 
            typeof(ValidationHelper), 
            new PropertyMetadata(default(Brush)));

        public static Brush GetBackground(DependencyObject element) => (Brush)element.GetValue(BackgroundProperty);

        public static void SetBackground(DependencyObject element, Brush value) => element.SetValue(BackgroundProperty, value);

        #endregion background

        #region font size

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.RegisterAttached(
            "FontSize", 
            typeof(double), 
            typeof(ValidationHelper), 
            new PropertyMetadata(10.0));

        public static double GetFontSize(DependencyObject element) => (double)element.GetValue(FontSizeProperty);

        public static void SetFontSize(DependencyObject element, double value) => element.SetValue(FontSizeProperty, value);

        #endregion font size
    }
}