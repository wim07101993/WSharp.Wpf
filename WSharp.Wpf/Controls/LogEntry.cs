using System.Windows;
using System.Windows.Controls;
using WSharp.Logging;

namespace WSharp.Wpf.Controls
{
    public class LogEntry : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(ILogEntry),
            typeof(LogEntry));

        static LogEntry()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogEntry), new FrameworkPropertyMetadata(typeof(LogEntry)));
        }

        public ILogEntry Value
        {
            get => (ILogEntry)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
