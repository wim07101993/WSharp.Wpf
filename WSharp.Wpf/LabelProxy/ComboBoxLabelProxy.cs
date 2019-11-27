using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WSharp.Wpf.LabelProxy
{
    internal sealed class ComboBoxLabelProxy : ALabelProxy
    {
        private readonly ComboBox _comboBox;
        private readonly TextChangedEventHandler _comboBoxTextChangedEventHandler;

        public ComboBoxLabelProxy(ComboBox comboBox) : base(comboBox)
        {
            _comboBox = comboBox ?? throw new ArgumentNullException(nameof(comboBox));
            _comboBoxTextChangedEventHandler = ComboBoxTextChanged;
            _comboBox.AddHandler(TextBoxBase.TextChangedEvent, _comboBoxTextChangedEventHandler);
            _comboBox.SelectionChanged += ComboBoxSelectionChanged;
            _comboBox.IsKeyboardFocusWithinChanged += ComboBoxIsKeyboardFocusWithinChanged;
        }

        public override object Content
        {
            get
            {
                if (_comboBox.IsEditable)
                {
                    return _comboBox.Text;
                }

                return _comboBox.SelectedItem is ComboBoxItem comboBoxItem
                    ? comboBoxItem.Content
                    : _comboBox.SelectedItem;
            }
        }

        public override bool IsEmpty() => string.IsNullOrEmpty(_comboBox.Text);

        public override bool IsFocused() => _comboBox.IsEditable && _comboBox.IsKeyboardFocusWithin;

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => _comboBox.Dispatcher.InvokeAsync(() => RaiseContentChanged(sender));

        private void ComboBoxTextChanged(object sender, TextChangedEventArgs e) => RaiseContentChanged(sender);

        private void ComboBoxIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) => RaiseFocusChanged(sender);

        protected override void Dispose(bool isDisposing)
        {
            _comboBox.RemoveHandler(TextBoxBase.TextChangedEvent, _comboBoxTextChangedEventHandler);
            _comboBox.SelectionChanged -= ComboBoxSelectionChanged;
            _comboBox.IsKeyboardFocusWithinChanged -= ComboBoxIsKeyboardFocusWithinChanged;
        }
    }
}
