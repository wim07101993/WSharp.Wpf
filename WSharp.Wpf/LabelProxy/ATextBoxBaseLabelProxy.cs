using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WSharp.Wpf.LabelProxy
{
    internal abstract class ATextBoxBaseLabelProxy : ALabelProxy
    {
        private readonly TextBoxBase _textBox;

        public ATextBoxBaseLabelProxy(TextBoxBase textBox) : base (textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _textBox.TextChanged += TextBoxTextChanged;
        }

        protected override void Dispose(bool isDisposing)
        {
            _textBox.TextChanged -= TextBoxTextChanged;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e) => RaiseContentChanged(sender);
    }
}