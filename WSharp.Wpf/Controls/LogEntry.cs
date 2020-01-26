using System.Windows;

using WSharp.Logging;

namespace WSharp.Wpf.Controls
{
    public class LogEntry : ValueControl<ILogEntry>
    {
        static LogEntry()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogEntry), new FrameworkPropertyMetadata(typeof(LogEntry)));
        }
    }
}
