using System.Windows;

namespace WSharp.Wpf.Helpers
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(object),
                typeof(BindingProxy),
                new PropertyMetadata(null));

        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
}