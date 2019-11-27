using System.Windows;
using System.Windows.Media;

namespace WSharp.Wpf.Helpers
{
    public static class LabelHelper
    {
        #region UseFloating

        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.RegisterAttached(
            "IsFloating",
            typeof(bool),
            typeof(LabelHelper),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetIsFloating(DependencyObject element) => (bool)element.GetValue(IsFloatingProperty);

        public static void SetIsFloating(DependencyObject element, bool value) => element.SetValue(IsFloatingProperty, value);

        #endregion UseFloating

        #region FloatingScale & FloatingOffset

        public static readonly DependencyProperty FloatingScaleProperty = DependencyProperty.RegisterAttached(
            "FloatingScale",
            typeof(double),
            typeof(LabelHelper),
            new FrameworkPropertyMetadata(0.74d, FrameworkPropertyMetadataOptions.Inherits));

        public static double GetFloatingScale(DependencyObject element) => (double)element.GetValue(FloatingScaleProperty);

        public static void SetFloatingScale(DependencyObject element, double value) => element.SetValue(FloatingScaleProperty, value);

        public static readonly DependencyProperty FloatingOffsetProperty = DependencyProperty.RegisterAttached(
            "FloatingOffset",
            typeof(Point),
            typeof(LabelHelper),
            new FrameworkPropertyMetadata(new Point(1, -16), FrameworkPropertyMetadataOptions.Inherits));

        public static Point GetFloatingOffset(DependencyObject element) => (Point)element.GetValue(FloatingOffsetProperty);

        public static void SetFloatingOffset(DependencyObject element, Point value) => element.SetValue(FloatingOffsetProperty, value);

        #endregion FloatingScale & FloatingOffset

        #region Label

        /// <summary>The hint property</summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label",
            typeof(object),
            typeof(LabelHelper),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Gets the hint.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static object GetLabel(DependencyObject element) => element.GetValue(LabelProperty);

        /// <summary>Sets the hint.</summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetLabel(DependencyObject element, object value) => element.SetValue(LabelProperty, value);

        #endregion Label

        #region LabelOpacity

        /// <summary>The hint opacity property</summary>
        public static readonly DependencyProperty LabelOpacityProperty = DependencyProperty.RegisterAttached(
            "LabelOpacity",
            typeof(double),
            typeof(LabelHelper),
            new PropertyMetadata(.56));

        /// <summary>Gets the text box view margin.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="Thickness"/>.</returns>
        public static double GetLabelOpacityProperty(DependencyObject element) => (double)element.GetValue(LabelOpacityProperty);

        /// <summary>Sets the hint opacity.</summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetLabelOpacity(DependencyObject element, double value) => element.SetValue(LabelOpacityProperty, value);

        #endregion LabelOpacity

        #region Brushes

        /// <summary>The color for the text of a focused control.</summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
            "Foreground", 
            typeof(Brush), 
            typeof(LabelHelper), 
            new PropertyMetadata(null));

        /// <summary>Gets the color for the text of a focused control.</summary>
        public static Brush GetForeground(DependencyObject element) => (Brush)element.GetValue(ForegroundProperty);

        /// <summary>Sets the color for the text of a focused control.</summary>
        public static void SetForeground(DependencyObject element, Brush value) => element.SetValue(ForegroundProperty, value);

        #endregion Brushes

        #region HelperText

        /// <summary>The HelperText property</summary>
        public static readonly DependencyProperty HelperTextProperty = DependencyProperty.RegisterAttached(
            "HelperText",
            typeof(string),
            typeof(LabelHelper),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Gets the HelperText.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static object GetHelperText(DependencyObject element) => element.GetValue(HelperTextProperty);

        /// <summary>Sets the HelperText.</summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetHelperText(DependencyObject element, object value) => element.SetValue(HelperTextProperty, value);

        #endregion HelperText
    }
}