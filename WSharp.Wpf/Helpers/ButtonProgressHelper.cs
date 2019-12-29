using System.Windows;
using System.Windows.Media;

namespace WSharp.Wpf.Helpers
{
    public static class ButtonProgressHelper
    {
        #region Minimum

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.RegisterAttached(
            "Minimum",
            typeof(double),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(double)));

        public static void SetMinimum(DependencyObject element, double value) => element.SetValue(MinimumProperty, value);

        public static double GetMinimum(DependencyObject element) => (double)element.GetValue(MinimumProperty);

        #endregion Minimum

        #region Maximum

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached(
            "Maximum",
            typeof(double),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(100.0));

        public static void SetMaximum(DependencyObject element, double value) => element.SetValue(MaximumProperty, value);

        public static double GetMaximum(DependencyObject element) => (double)element.GetValue(MaximumProperty);

        #endregion Maximum

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(double),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(double)));

        public static void SetValue(DependencyObject element, double value) => element.SetValue(ValueProperty, value);

        public static double GetValue(DependencyObject element) => (double)element.GetValue(ValueProperty);

        #endregion Value

        #region IsIndeterminate

        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.RegisterAttached(
            "IsIndeterminate",
            typeof(bool),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(bool)));

        public static void SetIsIndeterminate(DependencyObject element, bool isIndeterminate) => element.SetValue(IsIndeterminateProperty, isIndeterminate);

        public static bool GetIsIndeterminate(DependencyObject element) => (bool)element.GetValue(IsIndeterminateProperty);

        #endregion IsIndeterminate

        #region IndicatorForeground

        public static readonly DependencyProperty IndicatorForegroundProperty = DependencyProperty.RegisterAttached(
            "IndicatorForeground",
            typeof(Brush),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(Brush)));

        public static void SetIndicatorForeground(DependencyObject element, Brush indicatorForeground) => element.SetValue(IndicatorForegroundProperty, indicatorForeground);

        public static Brush GetIndicatorForeground(DependencyObject element) => (Brush)element.GetValue(IndicatorForegroundProperty);

        #endregion IndicatorForeground

        #region IndicatorBackground

        public static readonly DependencyProperty IndicatorBackgroundProperty = DependencyProperty.RegisterAttached(
            "IndicatorBackground",
            typeof(Brush),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(Brush)));

        public static void SetIndicatorBackground(DependencyObject element, Brush indicatorBackground) => element.SetValue(IndicatorBackgroundProperty, indicatorBackground);

        public static Brush GetIndicatorBackground(DependencyObject element) => (Brush)element.GetValue(IndicatorBackgroundProperty);

        #endregion IndicatorBackground

        #region IsIndicatorVisible

        public static readonly DependencyProperty IsIndicatorVisibleProperty = DependencyProperty.RegisterAttached(
            "IsIndicatorVisible",
            typeof(bool),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(bool)));

        public static void SetIsIndicatorVisible(DependencyObject element, bool isIndicatorVisible) => element.SetValue(IsIndicatorVisibleProperty, isIndicatorVisible);

        public static bool GetIsIndicatorVisible(DependencyObject element) => (bool)element.GetValue(IsIndicatorVisibleProperty);

        #endregion IsIndicatorVisible

        #region Opacity

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.RegisterAttached(
            "Opacity",
            typeof(double),
            typeof(ButtonProgressHelper),
            new FrameworkPropertyMetadata(default(double)));

        public static void SetOpacity(DependencyObject element, double opacity) => element.SetValue(OpacityProperty, opacity);

        public static double GetOpacity(DependencyObject element) => (double)element.GetValue(OpacityProperty);

        #endregion Opacity
    }
}
