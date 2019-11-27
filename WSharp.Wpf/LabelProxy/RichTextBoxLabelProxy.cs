using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WSharp.Wpf.LabelProxy
{
    internal sealed class RichTextBoxLabelProxy : ATextBoxBaseLabelProxy
    {
        private readonly RichTextBox _textBox;

        public override object Content => null;

        public RichTextBoxLabelProxy(RichTextBox textBox) : base(textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _textBox = textBox;
        }

        public override bool IsEmpty()
        {
            var textRange = new TextRange(
                _textBox.Document.ContentStart,
                _textBox.Document.ContentEnd
            );

            return string.IsNullOrWhiteSpace(textRange.Text);
        }
    }
}