using System;
using System.Windows;

namespace WSharp.Wpf.LabelProxy
{
    internal abstract class ALabelProxy : ILabelProxy
    {
        private readonly FrameworkElement _element;

        public ALabelProxy(FrameworkElement element)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _element.Loaded += TextBoxLoaded;
            _element.IsVisibleChanged += TextBoxIsVisibleChanged;
            _element.IsKeyboardFocusedChanged += TextBoxIsKeyboardFocusedChanged;
        }

        public abstract object Content { get; }

        public virtual bool IsLoaded => _element.IsLoaded;

        public virtual bool IsVisible => _element.IsVisible;

        public abstract bool IsEmpty();

        public virtual bool IsFocused() => _element.IsKeyboardFocused;

        public void Dispose()
        {
            _element.Loaded -= TextBoxLoaded;
            _element.IsVisibleChanged -= TextBoxIsVisibleChanged;
            _element.IsKeyboardFocusedChanged -= TextBoxIsKeyboardFocusedChanged;
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing) { }

        private void TextBoxIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e) => RaiseFocusChanged(sender);

        private void TextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void TextBoxLoaded(object sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        protected void RaiseContentChanged(object sender) => ContentChanged?.Invoke(sender, EventArgs.Empty);
        protected void RaiseFocusChanged(object sender) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        public event EventHandler ContentChanged;
        public event EventHandler IsVisibleChanged;
        public event EventHandler Loaded;
        public event EventHandler FocusedChanged;
    }
}
