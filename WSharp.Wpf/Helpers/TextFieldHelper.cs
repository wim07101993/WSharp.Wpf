using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Helpers
{
    /// <summary>Helper properties for working with text fields.</summary>
    public static class TextFieldHelper
    {
        #region view margin

        /// <summary>The text box view margin property</summary>
        public static readonly DependencyProperty TextBoxViewMarginProperty = DependencyProperty.RegisterAttached(
            "TextBoxViewMargin",
            typeof(Thickness),
            typeof(TextFieldHelper),
            new FrameworkPropertyMetadata(new Thickness(double.NegativeInfinity), FrameworkPropertyMetadataOptions.Inherits, TextBoxViewMarginPropertyChangedCallback));

        /// <summary>Gets the text box view margin.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="Thickness"/>.</returns>
        public static Thickness GetTextBoxViewMargin(DependencyObject element) => (Thickness)element.GetValue(TextBoxViewMarginProperty);

        /// <summary>Sets the text box view margin.</summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetTextBoxViewMargin(DependencyObject element, Thickness value) => element.SetValue(TextBoxViewMarginProperty, value);

        /// <summary>Applies the text box view margin.</summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="margin">The margin.</param>
        private static void ApplyTextBoxViewMargin(Control textBox, Thickness margin)
        {
            if (margin.Equals(new Thickness(double.NegativeInfinity)))
                return;

            if ((textBox.Template?.FindName("PART_ContentHost", textBox) as ScrollViewer)?.Content is FrameworkElement frameworkElement)
                frameworkElement.Margin = margin;
        }

        /// <summary>The text box view margin property changed callback.</summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">
        ///     The dependency property changed event args.
        /// </param>
        private static void TextBoxViewMarginPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Control box))
                return;

            if (box.IsLoaded)
                ApplyTextBoxViewMargin(box, (Thickness)dependencyPropertyChangedEventArgs.NewValue);

            box.Loaded += (sender, args) =>
            {
                var textBox = (Control)sender;
                ApplyTextBoxViewMargin(textBox, GetTextBoxViewMargin(textBox));
            };
        }

        #endregion view margin

        #region decoration visibility

        /// <summary>Controls the visibility of the underline decoration.</summary>
        public static readonly DependencyProperty DecorationVisibilityProperty = DependencyProperty.RegisterAttached(
            "DecorationVisibility",
            typeof(Visibility),
            typeof(TextFieldHelper),
            new PropertyMetadata(default(Visibility)));

        /// <summary>Controls the visibility of the underline decoration.</summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Visibility GetDecorationVisibility(DependencyObject element) => (Visibility)element.GetValue(DecorationVisibilityProperty);

        /// <summary>Controls the visibility of the underline decoration.</summary>
        public static void SetDecorationVisibility(DependencyObject element, Visibility value) => element.SetValue(DecorationVisibilityProperty, value);

        #endregion decoration visibility

    }
}