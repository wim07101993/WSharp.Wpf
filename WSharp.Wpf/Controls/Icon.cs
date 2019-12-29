using System.Windows;
using System.Windows.Controls;

namespace WSharp.Wpf.Controls
{
    /// <summary>Control that displays an icon.</summary>
    public class Icon : Control
    {
        /// <summary>Dependency property for the <see cref="Resource"/>.</summary>
        public static readonly DependencyProperty ResourceProperty = DependencyProperty.Register(
            nameof(Resource),
            typeof(string),
            typeof(Icon),
            new PropertyMetadata(default(string)));

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }

        /// <summary>The name of resource containing the path of the icon).</summary>
        public string Resource
        {
            get => (string)GetValue(ResourceProperty);
            set => SetValue(ResourceProperty, value);
        }
    }
}
