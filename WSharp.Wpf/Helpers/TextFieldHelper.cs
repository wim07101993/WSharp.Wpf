using System.Windows;
using System.Windows.Controls;
using WSharp.Wpf.Extensions;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Linq;

namespace WSharp.Wpf.Helpers
{
    /// <summary>Helper properties for working with text fields.</summary>
    public static class TextFieldHelper
    {
        #region TextBoxViewMargin

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

        #endregion TextBoxViewMargin

        #region DecorationVisibility

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

        #endregion DecorationVisibility

        #region UnderlineBrush

        /// <summary>
        /// The color for highlighting effects on the border of a text box.
        /// </summary>
        public static readonly DependencyProperty UnderlineBrushProperty = DependencyProperty.RegisterAttached(
            "UnderlineBrush", 
            typeof(Brush), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(null));

        /// <summary>
        /// Sets the color for highlighting effects on the border of a text box.
        /// </summary>
        public static void SetUnderlineBrush(DependencyObject element, Brush value) => element.SetValue(UnderlineBrushProperty, value);

        /// <summary>
        /// Gets the color for highlighting effects on the border of a text box.
        /// </summary>
        public static Brush GetUnderlineBrush(DependencyObject element) => (Brush)element.GetValue(UnderlineBrushProperty);

        #endregion UnderlineBrush

        #region TextFieldCornerRadius

        /// <summary>
        /// Controls the corner radius of the surrounding box.
        /// </summary>
        public static readonly DependencyProperty TextFieldCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "TextFieldCornerRadius", 
            typeof(CornerRadius), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(new CornerRadius(0.0)));

        public static void SetTextFieldCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(TextFieldCornerRadiusProperty, value);

        public static CornerRadius GetTextFieldCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(TextFieldCornerRadiusProperty);

        #endregion TextFieldCornerRadius

        #region RippleOnFocusEnabled

        /// <summary>
        /// Enables a ripple effect on focusing the text box.
        /// </summary>
        public static readonly DependencyProperty RippleOnFocusEnabledProperty = DependencyProperty.RegisterAttached(
            "RippleOnFocusEnabled", 
            typeof(bool), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(false));

        public static void SetRippleOnFocusEnabled(DependencyObject element, bool value) => element.SetValue(RippleOnFocusEnabledProperty, value);

        public static bool GetRippleOnFocusEnabled(DependencyObject element) => (bool)element.GetValue(RippleOnFocusEnabledProperty);

        #endregion RippleOnFocusEnabled

        #region SuffixText

        /// <summary>
        /// SuffixText dependency property
        /// </summary>
        public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.RegisterAttached(
            "SuffixText", 
            typeof(string), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(default(string)));

        public static void SetSuffixText(DependencyObject element, string value) => element.SetValue(SuffixTextProperty, value);

        public static string GetSuffixText(DependencyObject element) => (string)element.GetValue(SuffixTextProperty);

        #endregion SuffixText

        #region HasClearButton

        /// <summary>
        /// Controls the visbility of the clear button.
        /// </summary>
        public static readonly DependencyProperty HasClearButtonProperty = DependencyProperty.RegisterAttached(
            "HasClearButton", 
            typeof(bool), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(false, HasClearButtonChanged));

        private static void HasClearButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //could be a text box or password box
            if (!(d is Control box))
                return;
            
            if (box.IsLoaded)
                SetClearHandler(box);
            else
                box.Loaded += (sender, args) => SetClearHandler(box);
        }

        private static void SetClearHandler(Control box)
        {
            var bValue = GetHasClearButton(box);

            var clearButton = box.GetTemplateChild<Button>("PART_ClearButton");
            if (clearButton != null)
            {
                void Handler(object sender, RoutedEventArgs args)
                {
                    (box as TextBox)?.SetCurrentValue(TextBox.TextProperty, null);
                    (box as ComboBox)?.SetCurrentValue(ComboBox.TextProperty, null);
                    if (box is PasswordBox passwordBox)
                        passwordBox.Password = null;
                }

                if (bValue)
                    clearButton.Click += Handler;
                else
                    clearButton.Click -= Handler;
            }
        }

        public static void SetHasClearButton(DependencyObject element, bool value) => element.SetValue(HasClearButtonProperty, value);

        public static bool GetHasClearButton(DependencyObject element) => (bool)element.GetValue(HasClearButtonProperty);

        #endregion HasClearButton

        #region UnderlineCornerRadius

        /// <summary>
        /// Controls the corner radius of the bottom line of the surrounding box.
        /// </summary>
        public static readonly DependencyProperty UnderlineCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "UnderlineCornerRadius", 
            typeof(CornerRadius), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(new CornerRadius(0.0)));

        public static void SetUnderlineCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(UnderlineCornerRadiusProperty, value);

        public static CornerRadius GetUnderlineCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(UnderlineCornerRadiusProperty);

        #endregion UnderlineCornerRadius

        #region HasFilledTextField

        /// <summary>
        /// Controls the visibility of the filled text field.
        /// </summary>
        public static readonly DependencyProperty HasFilledTextFieldProperty = DependencyProperty.RegisterAttached(
            "HasFilledTextField", 
            typeof(bool), 
            typeof(TextFieldHelper),
            new PropertyMetadata(false));

        public static void SetHasFilledTextField(DependencyObject element, bool value) => element.SetValue(HasFilledTextFieldProperty, value);

        public static bool GetHasFilledTextField(DependencyObject element) => (bool)element.GetValue(HasFilledTextFieldProperty);

        #endregion HasFilledTextField

        #region HasOutlinedTextField

        /// <summary>
        /// Controls the visibility of the text field area box.
        /// </summary>
        public static readonly DependencyProperty HasOutlinedTextFieldProperty = DependencyProperty.RegisterAttached(
            "HasOutlinedTextField", 
            typeof(bool), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(false));

        public static void SetHasOutlinedTextField(DependencyObject element, bool value) => element.SetValue(HasOutlinedTextFieldProperty, value);

        public static bool GetHasOutlinedTextField(DependencyObject element) => (bool)element.GetValue(HasOutlinedTextFieldProperty);

        #endregion HasOutlinedTextField

        #region NewSpecHighlightingEnabled

        /// <summary>
        /// Controls the highlighting style of a text box.
        /// </summary>
        public static readonly DependencyProperty NewSpecHighlightingEnabledProperty = DependencyProperty.RegisterAttached(
            "NewSpecHighlightingEnabled", 
            typeof(bool), 
            typeof(TextFieldHelper), 
            new PropertyMetadata(false));

        public static void SetNewSpecHighlightingEnabled(DependencyObject element, bool value) => element.SetValue(NewSpecHighlightingEnabledProperty, value);

        public static bool GetNewSpecHighlightingEnabled(DependencyObject element) => (bool)element.GetValue(NewSpecHighlightingEnabledProperty);

        #endregion NewSpecHighlightingEnabled

        #region IncludeSpellingSuggestions

        /// <summary>
        /// Automatically inserts spelling suggestions into the text box context menu.
        /// </summary>
        public static readonly DependencyProperty IncludeSpellingSuggestionsProperty = DependencyProperty.RegisterAttached(
            "IncludeSpellingSuggestions",
            typeof(bool), 
            typeof(TextFieldHelper),
            new PropertyMetadata(default(bool), IncludeSpellingSuggestionsChanged));

        public static void SetIncludeSpellingSuggestions(TextBoxBase element, bool value) => element.SetValue(IncludeSpellingSuggestionsProperty, value);

        public static bool GetIncludeSpellingSuggestions(TextBoxBase element) => (bool)element.GetValue(IncludeSpellingSuggestionsProperty);

        private static void IncludeSpellingSuggestionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBoxBase textBox))
                return;
            
            if ((bool)e.NewValue)
            {
                textBox.ContextMenuOpening += TextBoxOnContextMenuOpening;
                textBox.ContextMenuClosing += TextBoxOnContextMenuClosing;
            }
            else
            {
                textBox.ContextMenuOpening -= TextBoxOnContextMenuOpening;
                textBox.ContextMenuClosing -= TextBoxOnContextMenuClosing;
            }
        }

        private static void TextBoxOnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!(sender is TextBoxBase textBox))
                return;

            var contextMenu = textBox?.ContextMenu;
            if (contextMenu == null)
                return;

            RemoveSpellingSuggestions(contextMenu);

            if (!SpellCheck.GetIsEnabled(textBox))
                return;

            var spellingError = GetSpellingError(textBox);
            if (spellingError != null)
            {
                var spellingSuggestionStyle =
                    contextMenu.TryFindResource(Spelling.SuggestionMenuItemStyleKey) as Style;

                var insertionIndex = 0;
                var hasSuggestion = false;
                foreach (var suggestion in spellingError.Suggestions)
                {
                    hasSuggestion = true;
                    var menuItem = new MenuItem
                    {
                        CommandTarget = textBox,
                        Command = EditingCommands.CorrectSpellingError,
                        CommandParameter = suggestion,
                        Style = spellingSuggestionStyle,
                        Tag = typeof(Spelling)
                    };
                    contextMenu.Items.Insert(insertionIndex++, menuItem);
                }
                if (!hasSuggestion)
                {
                    contextMenu.Items.Insert(insertionIndex++, new MenuItem
                    {
                        Style = contextMenu.TryFindResource(Spelling.NoSuggestionsMenuItemStyleKey) as Style,
                        Tag = typeof(Spelling)
                    });
                }

                contextMenu.Items.Insert(insertionIndex++, new Separator
                {
                    Style = contextMenu.TryFindResource(Spelling.SeparatorStyleKey) as Style,
                    Tag = typeof(Spelling)
                });

                contextMenu.Items.Insert(insertionIndex++, new MenuItem
                {
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandTarget = textBox,
                    Style = contextMenu.TryFindResource(Spelling.IgnoreAllMenuItemStyleKey) as Style,
                    Tag = typeof(Spelling)
                });

                contextMenu.Items.Insert(insertionIndex, new Separator
                {
                    Style = contextMenu.TryFindResource(Spelling.SeparatorStyleKey) as Style,
                    Tag = typeof(Spelling)
                });
            }
        }

        private static SpellingError GetSpellingError(TextBoxBase textBoxBase)
        {
            return textBoxBase switch
            {
                TextBox textBox => textBox.GetSpellingError(textBox.CaretIndex),
                RichTextBox richTextBox => richTextBox.GetSpellingError(richTextBox.CaretPosition),
                _ => null,
            };
        }

        private static void TextBoxOnContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            if (!(sender is TextBoxBase textBox))
                return;

            var contextMenu = textBox.ContextMenu;
            if (contextMenu != null)
                RemoveSpellingSuggestions(contextMenu);
        }

        private static void RemoveSpellingSuggestions(ContextMenu menu)
        {
            menu.Items.OfType<FrameworkElement>()
                .Where(x => ReferenceEquals(x.Tag, typeof(Spelling)))
                .ToList()
                .ForEach(menu.Items.Remove);
        }

        #endregion IncludeSpellingSuggestions
    }
}
