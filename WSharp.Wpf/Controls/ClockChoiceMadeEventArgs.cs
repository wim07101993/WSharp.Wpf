using System.Windows;

namespace WSharp.Wpf.Controls
{
    public class ClockChoiceMadeEventArgs : RoutedEventArgs
    {
        public ClockChoiceMadeEventArgs(EClockDisplayMode displayMode)
        {
            Mode = displayMode;
        }

        public ClockChoiceMadeEventArgs(EClockDisplayMode displayMode, RoutedEvent routedEvent) 
            : base(routedEvent)
        {
            Mode = displayMode;
        }

        public ClockChoiceMadeEventArgs(EClockDisplayMode displayMode, RoutedEvent routedEvent, object source) 
            : base(routedEvent, source)
        {
            Mode = displayMode;
        }

        public EClockDisplayMode Mode { get; }
    }
}
