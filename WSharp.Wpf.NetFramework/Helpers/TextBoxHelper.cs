using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WSharp.Wpf.NetFramework.Controls;
using WSharp.Wpf.NetFramework.Extensions;

namespace WSharp.Wpf.NetFramework.Helpers
{
    public static class TextBoxHelper
    {
        #region monitoring

        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached(
            "IsMonitoring",
            typeof(bool),
            typeof(TextBoxHelper),
            new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static void SetIsMonitoring(DependencyObject obj, bool value) => obj.SetValue(IsMonitoringProperty, value);

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (d)
            {
                case TextBox txtBox:
                    {
                        if ((bool)e.NewValue)
                        {
                            txtBox.BeginInvoke(() => TextChanged(txtBox, new TextChangedEventArgs(TextBoxBase.TextChangedEvent, UndoAction.None)));

                            txtBox.TextChanged += TextChanged;
                            txtBox.GotFocus += TextBoxGotFocus;
                        }
                        else
                        {
                            txtBox.TextChanged -= TextChanged;
                            txtBox.GotFocus -= TextBoxGotFocus;
                        }

                        break;
                    }

                case PasswordBox passBox:
                    {
                        if ((bool)e.NewValue)
                        {
                            passBox.BeginInvoke(() => PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox)));

                            passBox.PasswordChanged += PasswordChanged;
                            passBox.GotFocus += PasswordGotFocus;
                        }
                        else
                        {
                            passBox.PasswordChanged -= PasswordChanged;
                            passBox.GotFocus -= PasswordGotFocus;
                        }

                        break;
                    }

                case NumericUpDown spinner:
                    {
                        if ((bool)e.NewValue)
                        {
                            spinner.BeginInvoke(() => OnSpinnerValueChaged(spinner, new RoutedEventArgs(NumericUpDown.ValueChangedEvent, spinner)));
                            spinner.ValueChanged += OnSpinnerValueChaged;
                        }
                        else
                            spinner.ValueChanged -= OnSpinnerValueChaged;

                        break;
                    }

                case DatePicker datePicker:
                    {
                        if ((bool)e.NewValue)
                            datePicker.SelectedDateChanged += OnDatePickerBaseSelectedDateChanged;
                        else
                            datePicker.SelectedDateChanged -= OnDatePickerBaseSelectedDateChanged;

                        break;
                    }
            }
        }

        private static void TextChanged(object sender, RoutedEventArgs e) => SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
        private static void OnSpinnerValueChaged(object sender, RoutedEventArgs e) => SetTextLength(sender as NumericUpDown, numericUpDown => numericUpDown.Value.HasValue ? 1 : 0);
        private static void PasswordChanged(object sender, RoutedEventArgs e) => SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
        private static void OnDatePickerBaseSelectedDateChanged(object sender, RoutedEventArgs e) => SetTextLength(sender as DatePicker, timePickerBase => timePickerBase.SelectedDate.HasValue ? 1 : 0);
       
        private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> lengthSelector) where TDependencyObject : DependencyObject
        {
            if (sender == null)
                return;

            var value = lengthSelector(sender);
            sender.SetValue(TextLengthProperty, value);
            sender.SetValue(HasTextProperty, value >= 1);
        }

        #endregion monitoring

        #region text length

        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached(
            "TextLength",
            typeof(int),
            typeof(TextBoxHelper),
            new UIPropertyMetadata(0));

        #endregion text length

        #region has text

        public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached(
            "HasText",
            typeof(bool),
            typeof(TextBoxHelper),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets if the attached TextBox has text.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static bool GetHasText(DependencyObject obj) => (bool)obj.GetValue(HasTextProperty);

        public static void SetHasText(DependencyObject obj, bool value) => obj.SetValue(HasTextProperty, value);

        #endregion has text

        #region select all on focus

        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllOnFocus",
            typeof(bool),
            typeof(TextBoxHelper),
            new FrameworkPropertyMetadata(false));

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value) => obj.SetValue(SelectAllOnFocusProperty, value);
        public static bool GetSelectAllOnFocus(DependencyObject obj) => (bool)obj.GetValue(SelectAllOnFocusProperty);

        private static void TextBoxGotFocus(object sender, RoutedEventArgs e) => ControlGotFocus(sender as TextBox, textBox => textBox.SelectAll());
        private static void PasswordGotFocus(object sender, RoutedEventArgs e) => ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox.SelectAll());

        private static void ControlGotFocus<TDependencyObject>(TDependencyObject sender, Action<TDependencyObject> action)
            where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                if (GetSelectAllOnFocus(sender))
                {
                    sender.Dispatcher.BeginInvoke(action, sender);
                }
            }
        }

        #endregion select all on focus
    }
}
