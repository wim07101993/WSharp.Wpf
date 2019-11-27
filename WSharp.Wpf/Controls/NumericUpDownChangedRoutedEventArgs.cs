using System.Windows;

namespace WSharp.Wpf.Controls
{
    public class NumericUpDownChangedRoutedEventArgs : RoutedEventArgs
    {
        public NumericUpDownChangedRoutedEventArgs(double interval) 
        {
            Interval = interval;
        }
     
        public NumericUpDownChangedRoutedEventArgs(double interval, RoutedEvent routedEvent) : base(routedEvent)
        {
            Interval = interval;
        }
        
        public NumericUpDownChangedRoutedEventArgs(double interval, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            Interval = interval;
        }

        public double Interval { get; }
    }
}
