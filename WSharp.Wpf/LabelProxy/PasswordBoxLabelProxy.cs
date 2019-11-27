using System;
using System.Windows.Controls;

namespace WSharp.Wpf.LabelProxy
{
    internal sealed class PasswordBoxLabelProxy : ALabelProxy
    {
        private readonly PasswordBox _passwordBox;

        public override bool IsEmpty() => string.IsNullOrEmpty(_passwordBox.Password);

        public override object Content => _passwordBox.Password;

        public PasswordBoxLabelProxy(PasswordBox passwordBox) : base(passwordBox)
        {
            _passwordBox = passwordBox ?? throw new ArgumentNullException(nameof(passwordBox));
            _passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
        }

        private void PasswordBoxPasswordChanged(object sender, System.Windows.RoutedEventArgs e) => RaiseContentChanged(sender);

        protected override void Dispose(bool isDisposing)
        {
            _passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
        }
    }
}