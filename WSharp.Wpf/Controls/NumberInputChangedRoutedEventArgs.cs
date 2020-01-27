using System.Windows;

namespace WSharp.Wpf.Controls
{
    public class NumberInputChangedRoutedEventArgs : RoutedEventArgs
    {
        public double Interval { get; set; }

        public NumberInputChangedRoutedEventArgs(RoutedEvent routedEvent, double interval)
            : base(routedEvent)
        {
            Interval = interval;
        }
    }
}
