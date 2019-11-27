using System;
using System.Windows.Controls;

namespace WSharp.Wpf.LabelProxy
{
    internal sealed class TextBoxLabelProxy : ATextBoxBaseLabelProxy
    {
        private readonly TextBox _textBox;

        public TextBoxLabelProxy(TextBox textBox) : base(textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _textBox = textBox;
        }

        public override object Content => _textBox.Text;

        public override bool IsEmpty() => string.IsNullOrEmpty(_textBox.Text);
    }
}